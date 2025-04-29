using Microsoft.EntityFrameworkCore;
using SunBattery.Core.Entities;
using SunBattery_Api;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SunBattery_Api.Services.Commands
{
    public class ParseCommand : IParseCommand
    {
        private readonly ApplicationDbContext _dbContext;

        static MemoryStream _rxBuffer = new MemoryStream();
        static bool _gotResponse = false;

        public ParseCommand(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        enum ExitCode : int
        {
            Okay = 0,
            InvalidArgument = 1,
            NoResponse = 2,
            ResponseTooShort = 3,
            ResponseInvalidCrc = 4,

        }

        static void WriteUsage()
        {
            Console.WriteLine("Axpert reader");
            Console.WriteLine(" Usage:");
            Console.WriteLine("  axpert -p COM <-b [baud rate]> <-t [timeout ms]> command");
            Console.WriteLine();

        }

        /// <summary>
        /// Parses out command line args
        /// </summary>
        static string GetCommandLineArg(string[] args, string key)
        {
            string value = null;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == key)
                    value = args[i + 1];
            }
            return value;
        }

        /// <summary>
        /// Appends crc and CR bytes to a byte array
        /// </summary>
        static byte[] GetMessageBytes(string text)
        {
            //Get bytes for command
            byte[] command = Encoding.ASCII.GetBytes(text);

            //Get CRC for command bytes
            ushort crc = CalculateCrc(command);

            //Append CRC and CR to command
            byte[] result = new byte[command.Length + 3];
            command.CopyTo(result, 0);
            result[result.Length - 3] = (byte)(crc >> 8 & 0xFF);
            result[result.Length - 2] = (byte)(crc >> 0 & 0xFF);
            result[result.Length - 1] = 0x0d;

            return result;
        }

        /// <summary>
        /// Calculates CRC for axpert inverter
        /// Ported from crc.c: http://forums.aeva.asn.au/forums/pip4048ms-inverter_topic4332_page2.html
        /// </summary>
        static ushort CalculateCrc(byte[] pin)
        {
            ushort crc;
            byte da;
            byte ptr;
            byte bCRCHign;
            byte bCRCLow;

            int len = pin.Length;

            ushort[] crc_ta = new ushort[]
                {
                    0x0000,0x1021,0x2042,0x3063,0x4084,0x50a5,0x60c6,0x70e7,
                    0x8108,0x9129,0xa14a,0xb16b,0xc18c,0xd1ad,0xe1ce,0xf1ef
                };

            crc = 0;
            for (int index = 0; index < len; index++)
            {
                ptr = pin[index];

                da = (byte)((byte)(crc >> 8) >> 4);
                crc <<= 4;
                crc ^= crc_ta[da ^ ptr >> 4];
                da = (byte)((byte)(crc >> 8) >> 4);
                crc <<= 4;
                crc ^= crc_ta[da ^ ptr & 0x0f];
            }

            //Escape CR,LF,'H' characters
            bCRCLow = (byte)(crc & 0x00FF);
            bCRCHign = (byte)(crc >> 8);
            if (bCRCLow == 0x28 || bCRCLow == 0x0d || bCRCLow == 0x0a)
            {
                bCRCLow++;
            }
            if (bCRCHign == 0x28 || bCRCHign == 0x0d || bCRCHign == 0x0a)
            {
                bCRCHign++;
            }
            crc = (ushort)(bCRCHign << 8);
            crc |= bCRCLow;
            return crc;
        }

        static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = sender as SerialPort;

            if (sp != null && !_gotResponse)
            {
                //Read chars until we hit a CR character
                while (sp.BytesToRead > 0)
                {
                    byte b = (byte)sp.ReadByte();
                    _rxBuffer.WriteByte(b);

                    if (b == 0x0d)
                    {
                        _gotResponse = true;
                        break;
                    }
                }
            }
        }

        static void DataErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.Write(e.EventType);
        }

        public async Task ParseCommandStrAsync()
        {
            string[] args = [];

            int baud;
            int.TryParse(GetCommandLineArg(args, "-b") ?? "2400", out baud);

            int timeoutMs;
            int.TryParse(GetCommandLineArg(args, "-t") ?? "1000", out timeoutMs);

            string comPort = GetCommandLineArg(args, "-p");
            comPort = string.IsNullOrWhiteSpace(comPort) ? "COM3" : comPort;

            string commandText = args.Length == 0 ? null : args.Last();
            commandText = string.IsNullOrWhiteSpace(commandText) ? "QPIGS" : commandText;

            var infoTime = await _dbContext.InfoTimes.AsNoTracking().FirstOrDefaultAsync();
            var startTimeDay = infoTime?.StartTime;
            var endTimeDay = infoTime?.EndTime;

           

            if (infoTime == null)
            {
                TimeSpan start = new TimeSpan(10, 0, 0); //10 o'clock
                TimeSpan end = new TimeSpan(20, 0, 0); //12 o'clock
                infoTime = new InfoTime()
                {
                    StartTime = start,
                    EndTime = end,
                };


                await _dbContext.AddAsync(infoTime);
                _dbContext.SaveChanges();
            }
           
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (now == infoTime.StartTime)
            {
                commandText = string.IsNullOrWhiteSpace(commandText) ? "POP01" : commandText;
            }
            else if (now == infoTime.EndTime)
            {
                commandText = string.IsNullOrWhiteSpace(commandText) ? "POP00" : commandText;
            }
            //else
            //{
            //    commandText = string.IsNullOrWhiteSpace(commandText) ? "QPIGS" : commandText;
            //}
      

            SerialPort sp = new SerialPort();
            sp.PortName = comPort;
            sp.BaudRate = baud;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;

            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            sp.ErrorReceived += new SerialErrorReceivedEventHandler(DataErrorReceivedHandler);

            sp.Open();

            byte[] commandBytes = GetMessageBytes(commandText);

            //Flush out any existing chars
            sp.ReadExisting();

            //Send request
            sp.Write(commandBytes, 0, commandBytes.Length);

            //Wait for response
            var startTime = DateTime.Now;
            while (!_gotResponse && (DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                Thread.Sleep(20);
            }

            sp.Close();


            byte[] payloadBytes = new byte[_rxBuffer.Length - 3];
            Array.Copy(_rxBuffer.GetBuffer(), payloadBytes, payloadBytes.Length);

            ushort crcMsb = _rxBuffer.GetBuffer()[_rxBuffer.Length - 3];
            ushort crcLsb = _rxBuffer.GetBuffer()[_rxBuffer.Length - 2];

            ushort calculatedCrc = CalculateCrc(payloadBytes);
            ushort receivedCrc = (ushort)(crcMsb << 8 | crcLsb);

            //Write response to console
            string resultStr = Encoding.ASCII.GetString(payloadBytes);

            var values = resultStr.Trim('(').Split(' ');

            var res = SetData(values);

            await _dbContext.AddAsync(res);
            await _dbContext.SaveChangesAsync();


            for (var i = 0; i < values.Length; i++)
            {

                Console.WriteLine($"Index: {i}, \t\tValue: {values[i]}");
            }
            Console.WriteLine("-------------------");

        }

        public ProtocolData SetData(string[] list)
        {
            return new ProtocolData
            {
                Date = DateTime.UtcNow,
                GridVoltage = list[0],
                GridFrequency = list[1],
                ACOutputVoltage = list[2],
                ACOutputFrequency = list[3],
                ACOutputApparentPower = list[4],
                ACOutputActivePower = list[5],
                OutputLoadPercent = list[6],
                BUSVoltage = list[7],
                BatteryVoltage = list[8],
                BatteryChargingCurrent = list[9],
                BatteryCapacity = list[10],
                InverterHeatSinkTemperature = list[11],
                PVInputCurrent = list[12],
                PVInputVoltage = list[13],
                BatteryVoltageFromSCC = list[14],
                BatteryDischargeCurrent = list[15],
                DeviceStatus = list[16],
                BatteryVoltageOffsetForFansOn = list[17],
                EEPROMVersion = list[18],
                PVChargingPower = list[19],
                DeviceStatus2 = list[20],
            };

           
        }

        private ProtocolData SetData(ProtocolData data, int index)
        {

            return data;

        }
    }
}
