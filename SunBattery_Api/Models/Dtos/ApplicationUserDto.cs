namespace SunBattery_Api.Models.Dtos
{
    public class ApplicationUserDto : EntityDto
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool IsActive { get; set; }

        public string Email { get; set; }

        public required string RoleName { get; set; }
    }
}
