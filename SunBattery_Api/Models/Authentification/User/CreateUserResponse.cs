using SunBattery.Core.Entities;

namespace SunBattery_Api.Models.Authentification.User
{
    public class CreateUserResponse
    {
        public string Token { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}
