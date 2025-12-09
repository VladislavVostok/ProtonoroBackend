using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProtonoroBackend.DtoModels.Auth;
using ProtonoroBackend.Models;
using ProtonoroBackend.Services.Interfaces;
using System.Security.Claims;

namespace ProtonoroBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IProfileService _profileService;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;

        public AuthController(UserManager<AppUser> userManager,
                                ITokenService tokenService,
                                IEmailService emailService,
                                IAuthService authService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            Dictionary<string, string> res = await _authService.Login(loginDto);

            if (res.ContainsKey("accessToken"))
            {

                return Ok(res);
            }
            return Unauthorized(res);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            Dictionary<string, object> res = await _authService.Register(registerDto);
            if (res.ContainsKey("message"))
            {
                return Ok(res);
            }

            return BadRequest(res);
        }

        [HttpGet("password-reset-email/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordResetEmail(string email)
        {
            Dictionary<string, string> resp = await _authService.SendPasswordResetEmail(email);
            if (resp.ContainsKey("message"))
            {
                return Ok(resp);
            }
            return BadRequest(resp);
        }

        private string GenerateRandomOtp(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => random.Next(0, 10).ToString()[0]).ToArray());
        }

        [HttpPost("password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] PasswordResetDto resetDto)
        {
            Dictionary<string, string> resp = await _authService.ResetPassword(resetDto);

            if (resp.ContainsKey("message"))
            {
                return Ok(resp);
            }
            return NotFound(resp);
        }


        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changeDto)
        {
            Dictionary<string, string> resp = await _authService.ChangePassword(changeDto);
            string value = "";
            resp.TryGetValue("icon", out value);

            switch (value)
            {
                case "success":
                    return Ok(resp);
                case "warning":
                    return BadRequest((resp));
            }
            return NotFound(resp);
        }


        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized("No refresh token provided");

            string userId = null;

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                userId = identity?.FindFirst(ClaimTypes.UserData)?.Value;
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized("User not found");

            //TODO: Обратить внимание на логин провайдер вторым аргументом.
            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "JwtAuthDemo", "RefreshToken");
            if (storedToken != refreshToken)
                return Unauthorized("Invalid refresh token");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _userManager.SetAuthenticationTokenAsync(user, "JwtAuthDemo", "RefreshToken", newRefreshToken);
            SetRefreshTokenCookie(newRefreshToken);

            return Ok(new { accessToken = newAccessToken });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
