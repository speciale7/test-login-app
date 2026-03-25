using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LoginApi.Tests;

/// <summary>
/// Integration tests for AuthController endpoints:
/// register, login, refresh token, get current user, and logout.
/// </summary>
public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // Each test gets its own HttpClient to prevent auth header leakage between tests.
    private HttpClient CreateClient() => _factory.CreateClient();

    // ---- Helpers ----

    private static RegisterRequest ValidRegisterRequest(string suffix = "") => new()
    {
        Username = $"testuser{suffix}",
        Email = $"test{suffix}@example.com",
        Password = "Password1!"
    };

    private async Task<AuthResponse> RegisterAndGetTokensAsync(HttpClient client, string suffix = "")
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest(suffix));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions))!;
    }

    // ---- Register Tests ----

    [Fact]
    public async Task Register_ValidRequest_Returns200WithTokens()
    {
        var client = CreateClient();
        var request = ValidRegisterRequest("_reg1");
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEmpty(body.AccessToken);
        Assert.NotEmpty(body.RefreshToken);
        Assert.NotNull(body.User);
        Assert.Equal(request.Email, body.User.Email);
        Assert.Equal(request.Username, body.User.Username);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        var client = CreateClient();
        var request = ValidRegisterRequest("_dup_email");
        await client.PostAsJsonAsync("/api/auth/register", request);

        // Try registering again with the same email but different username
        var duplicate = new RegisterRequest
        {
            Username = "differentuser",
            Email = request.Email,
            Password = "Password1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/register", duplicate);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Returns400()
    {
        var client = CreateClient();
        var request = ValidRegisterRequest("_dup_user");
        await client.PostAsJsonAsync("/api/auth/register", request);

        var duplicate = new RegisterRequest
        {
            Username = request.Username,
            Email = "different_dup@example.com",
            Password = "Password1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/register", duplicate);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        var client = CreateClient();
        var request = new RegisterRequest
        {
            Username = "validuser_inv",
            Email = "not-an-email",
            Password = "Password1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_Returns400()
    {
        var client = CreateClient();
        var request = new RegisterRequest
        {
            Username = "weakpwuser",
            Email = "weakpw@example.com",
            Password = "weak"  // Too short, no uppercase, no number, no special char
        };
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_UsernameTooShort_Returns400()
    {
        var client = CreateClient();
        var request = new RegisterRequest
        {
            Username = "ab",  // Less than 3 chars
            Email = "shortname@example.com",
            Password = "Password1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---- Login Tests ----

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithTokens()
    {
        var client = CreateClient();
        await RegisterAndGetTokensAsync(client, "_login1");

        var request = new LoginRequest
        {
            Email = "test_login1@example.com",
            Password = "Password1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEmpty(body.AccessToken);
        Assert.NotEmpty(body.RefreshToken);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var client = CreateClient();
        await RegisterAndGetTokensAsync(client, "_login2");

        var request = new LoginRequest
        {
            Email = "test_login2@example.com",
            Password = "WrongPassword1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonExistentEmail_Returns401()
    {
        var client = CreateClient();
        var request = new LoginRequest
        {
            Email = "nobody@example.com",
            Password = "Password1!"
        };
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ---- Refresh Token Tests ----

    [Fact]
    public async Task RefreshToken_ValidToken_Returns200WithNewTokens()
    {
        var client = CreateClient();
        var auth = await RegisterAndGetTokensAsync(client, "_refresh1");

        var request = new RefreshTokenRequest { RefreshToken = auth.RefreshToken };
        var response = await client.PostAsJsonAsync("/api/auth/refresh", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotEmpty(body.AccessToken);
        Assert.NotEmpty(body.RefreshToken);
        // New tokens should differ from the old ones
        Assert.NotEqual(auth.AccessToken, body.AccessToken);
        Assert.NotEqual(auth.RefreshToken, body.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_Returns401()
    {
        var client = CreateClient();
        var request = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };
        var response = await client.PostAsJsonAsync("/api/auth/refresh", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_UsedTwice_SecondUsageReturns401()
    {
        var client = CreateClient();
        var auth = await RegisterAndGetTokensAsync(client, "_refresh2");

        var request = new RefreshTokenRequest { RefreshToken = auth.RefreshToken };

        // First use - should succeed
        var response1 = await client.PostAsJsonAsync("/api/auth/refresh", request);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        // Second use of same token - should fail (token is now revoked)
        var response2 = await client.PostAsJsonAsync("/api/auth/refresh", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response2.StatusCode);
    }

    // ---- GetCurrentUser (/me) Tests ----

    [Fact]
    public async Task GetCurrentUser_WithValidToken_Returns200WithUserInfo()
    {
        var client = CreateClient();
        var auth = await RegisterAndGetTokensAsync(client, "_me1");

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("test_me1@example.com", body.Email);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_Returns401()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithInvalidToken_Returns401()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid.token.here");

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ---- Logout Tests ----

    [Fact]
    public async Task Logout_WithValidToken_Returns200()
    {
        var client = CreateClient();
        var auth = await RegisterAndGetTokensAsync(client, "_logout1");

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new RefreshTokenRequest { RefreshToken = auth.RefreshToken };
        var response = await client.PostAsJsonAsync("/api/auth/logout", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Logout_RefreshTokenIsRevokedAfterLogout()
    {
        var client = CreateClient();
        var auth = await RegisterAndGetTokensAsync(client, "_logout2");

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Logout
        var logoutRequest = new RefreshTokenRequest { RefreshToken = auth.RefreshToken };
        await client.PostAsJsonAsync("/api/auth/logout", logoutRequest);

        // Attempt to refresh with the now-revoked token using a fresh client
        var freshClient = CreateClient();
        var refreshRequest = new RefreshTokenRequest { RefreshToken = auth.RefreshToken };
        var refreshResponse = await freshClient.PostAsJsonAsync("/api/auth/refresh", refreshRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }
}
