using ProtonoroBackend.Models;
using System.Security.Claims;

namespace ProtonoroBackend.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}