namespace SunBattery_Api.Models.Dtos.ProtocolData
{
    public class ProtocolDataDto : EntityDto
    {
        public DateTime Date { get; set; }
        public string? GridVoltage { get; set; }
        public string? GridFrequency { get; set; }

        public string? ACOutputApparentPower { get; set; }
        public string? ACOutputActivePower { get; set; }
        public string? OutputLoadPercent { get; set; }

        public string? DeviceStatus2 { get; set; }
    }
}
