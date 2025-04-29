namespace SunBattery_Api.Models.Authentification.User
{
    public class LoginResponse
    {
        public TokenType AccessToken { get; set; }
        public TokenType RefreshToken { get; set; }
    }
}
