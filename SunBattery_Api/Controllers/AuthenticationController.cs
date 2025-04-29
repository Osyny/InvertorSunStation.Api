using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SunBattery.Core.Entities;
using SunBattery_Api.Models;
using SunBattery_Api.Models.Authentification.Login;
using SunBattery_Api.Models.Authentification.SignUp;
using SunBattery_Api.Models.EmailSenderModels;
using SunBattery_Api.Services.EmailServices;
using SunBattery_Api.Services.UserManagements;

namespace SunBattery_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IEmailService _emailService;
        private readonly IUserManagement _userManagement;
        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            SignInManager<ApplicationUser> signInManager,
            IUserManagement userManagement
            )
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _userManagement = userManagement;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var tokenResponse = await _userManagement.CreateUserWithTokenAsync(registerUser);
            if (tokenResponse.IsSuccess && tokenResponse.Response != null)
            {
                await _userManagement.AssignRoleToUserAsync(registerUser.Roles, tokenResponse.Response.User);

                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication",
                    new { tokenResponse.Response.Token, email = registerUser.Email }, Request.Scheme);

                //var confirmationLink = $"http://localhost:4200/confirm-account?Token={HttpUtility.UrlEncode(tokenResponse.Response.Token)}&email={HttpUtility.UrlEncode(registerUser.Email)}";

                var message = new Message(new string[] { registerUser.Email! }, "Confirmation email link", confirmationLink!);
                var responseMsg = _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                        new Response { IsSuccess = true, Message = $"{tokenResponse.Message} {responseMsg}" });
            }

            return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response { Message = tokenResponse.Message, IsSuccess = false });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var loginOtpResponse = await _userManagement.GetOtpByLoginAsync(loginModel);
            if (loginOtpResponse.Response != null)
            {
                var user = loginOtpResponse.Response.User;
                if (user.TwoFactorEnabled)
                {
                    var token = loginOtpResponse.Response.Token;
                    var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                     new Response { IsSuccess = loginOtpResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }

                var isValidPass = await _userManager.CheckPasswordAsync(user, loginModel.Password);
                if (user != null && isValidPass)
                {
                    var serviceResponse = await _userManagement.GetJwtTokenAsync(user);
                    return Ok(serviceResponse);
                }
            }
            return Unauthorized();

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist != null)
            {
                var result = await _userManager.ConfirmEmailAsync(userExist, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                           new Response { Status = "Success", Message = "Email Verified Succcessfully!" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
            new Response { Status = "Error", Message = "This User Doesnot exist!" });
        }

    }
}
