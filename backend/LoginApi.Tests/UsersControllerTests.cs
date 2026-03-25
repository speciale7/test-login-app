using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace LoginApi.Tests;

/// <summary>
/// Integration tests for UsersController endpoints,
/// verifying admin-only access and CRUD operations.
/// </summary>
public class UsersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UsersControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() => _factory.CreateClient();

    // ---- Helpers ----

    private async Task<(string AccessToken, int UserId)> RegisterUserAsync(
        HttpClient client, string username, string email, string password = "Password1!")
    {
        var request = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password
        };
        var response = await client.PostAsJsonAsync("/api/auth/register", request);
        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        return (auth!.AccessToken, auth.User.Id);
    }

    /// <summary>
    /// Creates an admin user using a fresh factory instance so the test is isolated.
    /// Promotes the first registered user to Admin by directly updating the in-memory DB,
    /// then logs back in to get a token that contains the Admin role claim.
    /// </summary>
    private async Task<(HttpClient AdminClient, string AdminToken)> CreateAdminClientAsync(string suffix)
    {
        var factory = new TestWebApplicationFactory();
        var client = factory.CreateClient();

        var registerRequest = new RegisterRequest
        {
            Username = $"admin_{suffix}",
            Email = $"admin_{suffix}@example.com",
            Password = "Admin1!Pass"
        };
        var regResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        regResponse.EnsureSuccessStatusCode();
        var auth = await regResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);

        // Promote user to Admin via direct DB access
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LoginApi.Data.AppDbContext>();
            var user = await db.Users.FindAsync(auth!.User.Id);
            if (user != null)
            {
                user.Role = UserRole.Admin;
                await db.SaveChangesAsync();
            }
        }

        // Log back in to get a fresh token reflecting the Admin role
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginAuth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        var adminToken = loginAuth!.AccessToken;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        return (client, adminToken);
    }

    // ---- Authorization Tests ----

    [Fact]
    public async Task GetUsers_Unauthenticated_Returns401()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ReaderRole_Returns403()
    {
        var client = CreateClient();
        var (token, _) = await RegisterUserAsync(client, "readeruser_list", "reader_list@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_AdminRole_Returns200()
    {
        var (adminClient, _) = await CreateAdminClientAsync("getusers");
        var response = await adminClient.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_Unauthenticated_Returns401()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/users/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_AdminRole_NonExistentId_Returns404()
    {
        var (adminClient, _) = await CreateAdminClientAsync("getuser404");
        var response = await adminClient.GetAsync("/api/users/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---- Delete Tests ----

    [Fact]
    public async Task DeleteUser_ReaderRole_Returns403()
    {
        var client = CreateClient();
        var (token, _) = await RegisterUserAsync(client, "reader_del", "reader_del@example.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync("/api/users/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_AdminCannotDeleteSelf_Returns400()
    {
        var factory = new TestWebApplicationFactory();
        var client = factory.CreateClient();

        var registerRequest = new RegisterRequest
        {
            Username = "selfdelete_admin",
            Email = "selfdelete@example.com",
            Password = "Admin1!Pass"
        };
        var regResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        regResponse.EnsureSuccessStatusCode();
        var auth = await regResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);

        // Promote to admin
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LoginApi.Data.AppDbContext>();
            var user = await db.Users.FindAsync(auth!.User.Id);
            if (user != null)
            {
                user.Role = UserRole.Admin;
                await db.SaveChangesAsync();
            }
        }

        // Log back in to get fresh token
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginAuth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginAuth!.AccessToken);

        // Attempt to delete own account
        var response = await client.DeleteAsync($"/api/users/{loginAuth.User.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
