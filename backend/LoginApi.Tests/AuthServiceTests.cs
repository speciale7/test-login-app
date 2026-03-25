using LoginApi.Models;
using LoginApi.Services;
using Microsoft.Extensions.Configuration;

namespace LoginApi.Tests;

/// <summary>
/// Unit tests for AuthService - password hashing and JWT token operations.
/// </summary>
public class AuthServiceTests
{
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["JwtSettings:SecretKey"] = "TestSecretKeyForUnitTestsMustBe32CharactersLong!!",
            ["JwtSettings:Issuer"] = "TestIssuer",
            ["JwtSettings:Audience"] = "TestAudience",
            ["JwtSettings:AccessTokenExpirationMinutes"] = "15",
            ["JwtSettings:RefreshTokenExpirationDays"] = "7"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new AuthService(configuration);
    }

    private static User CreateTestUser(int id = 1) => new()
    {
        Id = id,
        Username = "testuser",
        Email = "test@example.com",
        PasswordHash = string.Empty,
        Role = UserRole.Reader
    };

    // --- Password Hashing Tests ---

    [Fact]
    public void HashPassword_ValidPassword_ReturnsNonNullHash()
    {
        var hash = _authService.HashPassword("Password1!");
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_SamePasswordTwice_ReturnsDifferentHashes()
    {
        // BCrypt uses a random salt, so hashes should differ
        var hash1 = _authService.HashPassword("Password1!");
        var hash2 = _authService.HashPassword("Password1!");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "Password1!";
        var hash = _authService.HashPassword(password);
        Assert.True(_authService.VerifyPassword(password, hash));
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = _authService.HashPassword("Password1!");
        Assert.False(_authService.VerifyPassword("WrongPassword!", hash));
    }

    [Fact]
    public void VerifyPassword_EmptyPassword_ReturnsFalse()
    {
        var hash = _authService.HashPassword("Password1!");
        Assert.False(_authService.VerifyPassword(string.Empty, hash));
    }

    // --- Access Token Tests ---

    [Fact]
    public void GenerateAccessToken_ValidUser_ReturnsNonEmptyToken()
    {
        var user = CreateTestUser();
        var token = _authService.GenerateAccessToken(user);
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateAccessToken_ValidUser_ReturnsJwtFormat()
    {
        var user = CreateTestUser();
        var token = _authService.GenerateAccessToken(user);
        // JWT tokens have exactly 3 parts separated by dots
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void GenerateAccessToken_DifferentUsers_ReturnsDifferentTokens()
    {
        var user1 = CreateTestUser(1);
        var user2 = CreateTestUser(2);
        user2.Username = "anotheruser";
        user2.Email = "another@example.com";

        var token1 = _authService.GenerateAccessToken(user1);
        var token2 = _authService.GenerateAccessToken(user2);
        Assert.NotEqual(token1, token2);
    }

    // --- Refresh Token Tests ---

    [Fact]
    public void GenerateRefreshToken_ValidUserId_ReturnsActiveToken()
    {
        var refreshToken = _authService.GenerateRefreshToken(1);
        Assert.NotNull(refreshToken);
        Assert.True(refreshToken.IsActive);
        Assert.False(refreshToken.IsRevoked);
        Assert.False(refreshToken.IsExpired);
    }

    [Fact]
    public void GenerateRefreshToken_ValidUserId_TokenExpiresInFuture()
    {
        var refreshToken = _authService.GenerateRefreshToken(1);
        Assert.True(refreshToken.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateRefreshToken_ValidUserId_HasCorrectUserId()
    {
        var userId = 42;
        var refreshToken = _authService.GenerateRefreshToken(userId);
        Assert.Equal(userId, refreshToken.UserId);
    }

    [Fact]
    public void GenerateRefreshToken_CalledTwice_ReturnsDifferentTokens()
    {
        var token1 = _authService.GenerateRefreshToken(1);
        var token2 = _authService.GenerateRefreshToken(1);
        Assert.NotEqual(token1.Token, token2.Token);
    }

    // --- Token Validation Tests ---

    [Fact]
    public void ValidateAccessToken_ValidToken_ReturnsCorrectUserId()
    {
        var user = CreateTestUser(7);
        var token = _authService.GenerateAccessToken(user);
        var result = _authService.ValidateAccessToken(token);
        Assert.Equal(7, result);
    }

    [Fact]
    public void ValidateAccessToken_InvalidToken_ReturnsNull()
    {
        var result = _authService.ValidateAccessToken("this.is.not.a.valid.jwt.token");
        Assert.Null(result);
    }

    [Fact]
    public void ValidateAccessToken_EmptyToken_ReturnsNull()
    {
        var result = _authService.ValidateAccessToken(string.Empty);
        Assert.Null(result);
    }

    [Fact]
    public void ValidateAccessToken_TamperedToken_ReturnsNull()
    {
        var user = CreateTestUser(1);
        var token = _authService.GenerateAccessToken(user);
        // Tamper with the signature (last part of JWT)
        var parts = token.Split('.');
        parts[2] = "tampered_signature";
        var tamperedToken = string.Join('.', parts);
        var result = _authService.ValidateAccessToken(tamperedToken);
        Assert.Null(result);
    }
}
