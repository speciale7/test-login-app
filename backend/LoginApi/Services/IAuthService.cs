using LoginApi.Models;

namespace LoginApi.Services;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(int userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    int? ValidateAccessToken(string token);
}
