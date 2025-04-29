namespace SunBattery.Core.Entities
{
    public class ProtocolData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? GridVoltage { get; set; }
        public string? GridFrequency { get; set; }
        public string? ACOutputVoltage { get; set; }
        public string? ACOutputFrequency { get; set; }
        public string? ACOutputApparentPower { get; set; }
        public string? ACOutputActivePower { get; set; }
        public string? OutputLoadPercent { get; set; }
        public string? BUSVoltage { get; set; }
        public string? BatteryVoltage { get; set; }
        public string? BatteryChargingCurrent { get; set; }
        public string? BatteryCapacity { get; set; }
        public string? InverterHeatSinkTemperature  { get; set; }
        public string? PVInputCurrent  { get; set; }
        public string? PVInputVoltage { get; set; }
        public string? BatteryVoltageFromSCC { get; set; }
        public string? BatteryDischargeCurrent { get; set; }
        public string? DeviceStatus { get; set; }
        public string? BatteryVoltageOffsetForFansOn { get; set; }
        public string? EEPROMVersion { get; set; }
        public string? PVChargingPower { get; set; }
        public string? DeviceStatus2 { get; set; }
    }
}
