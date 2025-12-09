using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ProtonoroBackend.Configurations;
using ProtonoroBackend.DtoModels.Auth;
using ProtonoroBackend.Models;
using ProtonoroBackend.Services.Interfaces;

namespace ProtonoroBackend.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IProfileService _profileService;
        private readonly IEmailService _emailService;
        private readonly FrontendServerSettings _frontServerSettings;

        public AuthService(UserManager<AppUser> userManager,
                                ITokenService tokenService,
                                IEmailService emailService,
                                //IProfileService profileService,
                                IOptions<FrontendServerSettings> frontendServerSettings
            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            //_profileService = profileService;
            //_frontServerSettings = frontendServerSettings.Value;
        }


        public async Task<Dictionary<string, string>> Login(LoginDto loginDto)
        {

            Dictionary<string, string> resp = new Dictionary<string, string>();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var _accessToken = _tokenService.GenerateAccessToken(user);
                var _refreshToken = _tokenService.GenerateRefreshToken();


                resp.Add("accessToken", _accessToken);
                resp.Add("refreshToken", _refreshToken);

                // Сохраните refreshToken в базе данных или другой системе
                user.RefreshToken = _refreshToken;
                await _userManager.UpdateAsync(user);

                return resp;
            }

            resp.Add("Message", "Invalid credentials");

            return resp;
        }


        public async Task<Dictionary<string, object>> Register(RegisterDto registerDto)
        {

            Dictionary<string, object> resp = new Dictionary<string, object>();
            AppUser user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                FullName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                //_profileService.CreateProfileForUser(user);
                resp.Add("message", "User registered successfully");
                return resp;
            }

            resp.Add("error", result.Errors);
            return resp;
        }


        public async Task<Dictionary<string, string>> SendPasswordResetEmail(string email)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.OTP = GenerateRandomOtp(7);
                user.RefreshToken = _tokenService.GenerateRefreshToken(); // Simplified refresh token generation
                await _userManager.UpdateAsync(user);

                var link = $"http://{_frontServerSettings.Host}:{_frontServerSettings.Port}/password-reset?otp={user.OTP}&uuid={user.Id}&refresh_token={user.RefreshToken}";


                await _emailService.SendEmailAsync(user.Email, "Восстановление пароля", link);

                Console.WriteLine($"Reset Link: {link}");

                resp.Add("message", "If the email exists, a reset link was sent.");

                return resp;
            }

            resp.Add("error", "Такого email не существует!");

            return resp;
        }

        private string GenerateRandomOtp(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => random.Next(0, 10).ToString()[0]).ToArray());
        }

        public async Task<Dictionary<string, string>> ResetPassword(PasswordResetDto resetDto)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();
            var user = await _userManager.FindByIdAsync(resetDto.Uuid);
            if (user != null && user.OTP == resetDto.OTP)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddPasswordAsync(user, resetDto.Password);
                    user.OTP = null; // Clear the OTP
                    await _userManager.UpdateAsync(user);

                    resp.Add("message", "Password changed successfully");
                    return resp;
                }
            }

            resp.Add("error", "Invalid OTP or user not found");
            return resp;
        }

        public async Task<Dictionary<string, string>> ChangePassword(ChangePasswordDto changeDto)
        {
            Dictionary<string, string> resp = new Dictionary<string, string>();

            var user = await _userManager.FindByIdAsync(changeDto.UserId);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, changeDto.OldPassword, changeDto.NewPassword);
                if (result.Succeeded)
                {
                    resp.Add("message", "Password changed successfully");
                    resp.Add("icon", "success");
                    return resp;
                }
                resp.Add("message", "Old password is incorrect");
                resp.Add("icon", "warning");
                return resp;
            }

            resp.Add("message", "User does not exist");
            resp.Add("icon", "error");

            return resp;
        }

    }
}
