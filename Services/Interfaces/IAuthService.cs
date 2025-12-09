using ProtonoroBackend.DtoModels.Auth;

namespace ProtonoroBackend.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<Dictionary<string, string>> Login(LoginDto loginDto);
        public Task<Dictionary<string, object>> Register(RegisterDto registerDto);
        public Task<Dictionary<string, string>> SendPasswordResetEmail(string email);
        public Task<Dictionary<string, string>> ResetPassword(PasswordResetDto resetDto);
        public Task<Dictionary<string, string>> ChangePassword(ChangePasswordDto changeDto);
    }
}
