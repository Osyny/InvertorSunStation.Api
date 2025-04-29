using SunBattery.Core.Entities;
using SunBattery_Api.Models;
using SunBattery_Api.Models.Authentification.Login;
using SunBattery_Api.Models.Authentification.SignUp;
using SunBattery_Api.Models.Authentification.User;

namespace SunBattery_Api.Services.UserManagements
{
    public interface IUserManagement
    {

        /// <summary>
        /// Brief description of what the method does.
        /// </summary>
        /// <param name="registerUser">Description of the parameter.</param>
        /// <returns>Description of the return value.</returns>

        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerUser);
        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user);
        Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(LoginModel loginModel);
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
      //  Task<ApiResponse<LoginResponse>> LoginUser2FactorSignInWithJWTokenAsync(string otp, string userName);
        Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens);
    }
}
