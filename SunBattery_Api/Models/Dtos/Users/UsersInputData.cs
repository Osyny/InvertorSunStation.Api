namespace SunBattery_Api.Models.Dtos.Users
{
    public class UsersInputData
    {

        public int Rows { get; set; }
        public int Skip { get; set; }

        public string? Sorting { get; set; }
        public string? FilterText { get; set; }
    }
}
