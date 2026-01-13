# Prompt Copilot per Ricreare lo Stack Completo

Questa raccolta di prompt permette di ricreare l'intera applicazione full-stack di autenticazione con .NET 8, Angular 18, JWT, e Docker.

---

## 📋 INDICE

1. [Prompt 1: Setup Iniziale del Progetto](#prompt-1-setup-iniziale-del-progetto)
2. [Prompt 2: Backend - Modelli e Database](#prompt-2-backend---modelli-e-database)
3. [Prompt 3: Backend - Servizi di Autenticazione](#prompt-3-backend---servizi-di-autenticazione)
4. [Prompt 4: Backend - Controller API](#prompt-4-backend---controller-api)
5. [Prompt 5: Backend - Configurazione e Program.cs](#prompt-5-backend---configurazione-e-programcs)
6. [Prompt 6: Frontend - Setup Angular e Modelli](#prompt-6-frontend---setup-angular-e-modelli)
7. [Prompt 7: Frontend - Servizi e Interceptor](#prompt-7-frontend---servizi-e-interceptor)
8. [Prompt 8: Frontend - Guards e Routing](#prompt-8-frontend---guards-e-routing)
9. [Prompt 9: Frontend - Componenti UI](#prompt-9-frontend---componenti-ui)
10. [Prompt 10: Docker e Deployment](#prompt-10-docker-e-deployment)

---

## PROMPT 1: Setup Iniziale del Progetto

```
Crea la struttura iniziale di un'applicazione full-stack per l'autenticazione con le seguenti specifiche:

BACKEND (.NET 8 Web API):
- Crea un nuovo progetto ASP.NET Core Web API chiamato "LoginApi" nella cartella "backend/LoginApi"
- Aggiungi i seguenti pacchetti NuGet:
  * Microsoft.EntityFrameworkCore (8.0.0)
  * Microsoft.EntityFrameworkCore.Sqlite (8.0.0)
  * Microsoft.EntityFrameworkCore.Tools (8.0.0)
  * Microsoft.AspNetCore.Authentication.JwtBearer (8.0.0)
  * BCrypt.Net-Next (4.0.3)
  * Swashbuckle.AspNetCore (6.5.0)

FRONTEND (Angular 18):
- Crea un nuovo progetto Angular 18 standalone nella cartella "frontend"
- Usa il routing
- Usa il CSS come stile
- Aggiungi Angular Material 18 con il tema Indigo/Pink

STRUTTURA CARTELLE BACKEND:
backend/LoginApi/
├── Controllers/
├── Data/
├── DTOs/
├── Models/
├── Services/
├── Migrations/
└── Properties/

STRUTTURA CARTELLE FRONTEND:
frontend/src/app/
├── components/
│   ├── login/
│   ├── register/
│   ├── dashboard/
│   └── navigation/
├── services/
├── guards/
├── interceptors/
└── models/

Crea anche:
- File README.md nella root
- File .gitignore per .NET e Angular
- File docker-compose.yml (vuoto per ora)
```

---

## PROMPT 2: Backend - Modelli e Database

```
Crea i modelli del database e il DbContext per l'applicazione di autenticazione:

FILE: backend/LoginApi/Models/UserRole.cs
```csharp
namespace LoginApi.Models;

/// <summary>
/// Enumeration of user roles in the system
/// </summary>
public enum UserRole
{
    Reader = 0,
    Operator = 1,
    Admin = 2
}
```

FILE: backend/LoginApi/Models/User.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    public UserRole Role { get; set; } = UserRole.Reader;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property for refresh tokens
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
```

FILE: backend/LoginApi/Models/RefreshToken.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public int UserId { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsRevoked { get; set; } = false;
    
    public DateTime? RevokedAt { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
    
    // Helper property to check if token is still active
    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
}
```

FILE: backend/LoginApi/Data/AppDbContext.cs
```csharp
using LoginApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Data;

/// <summary>
/// Database context for the application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(u => u.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            
            entity.Property(rt => rt.Token).IsRequired();
            entity.Property(rt => rt.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relationship: RefreshToken belongs to User
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
```

Dopo aver creato questi file, esegui:
```bash
cd backend/LoginApi
dotnet ef migrations add InitialCreate
dotnet ef database update
```
```

---

## PROMPT 3: Backend - Servizi di Autenticazione

```
Crea i servizi di autenticazione per gestire JWT, hashing delle password e refresh token:

FILE: backend/LoginApi/Services/IAuthService.cs
```csharp
using LoginApi.Models;

namespace LoginApi.Services;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    string GenerateAccessToken(User user);
    
    /// <summary>
    /// Generates a refresh token for the user
    /// </summary>
    RefreshToken GenerateRefreshToken(int userId);
    
    /// <summary>
    /// Hashes a password using BCrypt
    /// </summary>
    string HashPassword(string password);
    
    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
    
    /// <summary>
    /// Validates an access token and returns the user ID if valid
    /// </summary>
    int? ValidateAccessToken(string token);
}
```

FILE: backend/LoginApi/Services/AuthService.cs
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginApi.Models;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace LoginApi.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Generates a JWT access token for the user
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    /// <summary>
    /// Generates a refresh token for the user
    /// </summary>
    public RefreshToken GenerateRefreshToken(int userId)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshTokenExpirationDays = Convert.ToInt32(jwtSettings["RefreshTokenExpirationDays"]);
        
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Hashes a password using BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }
    
    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    
    /// <summary>
    /// Validates an access token and returns the user ID if valid
    /// </summary>
    public int? ValidateAccessToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}
```
```

---

## PROMPT 4: Backend - Controller API

```
Crea i DTOs e il controller API per l'autenticazione:

FILE: backend/LoginApi/DTOs/AuthDtos.cs
```csharp
using System.ComponentModel.DataAnnotations;
using LoginApi.Models;

namespace LoginApi.DTOs;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
    [MaxLength(50, ErrorMessage = "Username must not exceed 50 characters")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(100, ErrorMessage = "Email must not exceed 100 characters")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// Request model for refreshing access token
/// </summary>
public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Response model for authentication
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
}

/// <summary>
/// Response model for user information
/// </summary>
public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

FILE: backend/LoginApi/Controllers/AuthController.cs
```csharp
using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using LoginApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(AppDbContext context, IAuthService authService, ILogger<AuthController> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already registered" });
            }
            
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already taken" });
            }
            
            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _authService.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Generate tokens
            var accessToken = _authService.GenerateAccessToken(user);
            var refreshToken = _authService.GenerateRefreshToken(user.Id);
            
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User registered successfully: {Email}", user.Email);
            
            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }
    
    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            
            // Generate tokens
            var accessToken = _authService.GenerateAccessToken(user);
            var refreshToken = _authService.GenerateRefreshToken(user.Id);
            
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User logged in successfully: {Email}", user.Email);
            
            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }
    
    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
            
            if (refreshToken == null || !refreshToken.IsActive)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }
            
            // Generate new tokens
            var accessToken = _authService.GenerateAccessToken(refreshToken.User);
            var newRefreshToken = _authService.GenerateRefreshToken(refreshToken.UserId);
            
            // Revoke old refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            
            // Add new refresh token
            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();
            
            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserResponse
                {
                    Id = refreshToken.User.Id,
                    Username = refreshToken.User.Username,
                    Email = refreshToken.User.Email,
                    Role = refreshToken.User.Role,
                    CreatedAt = refreshToken.User.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }
    
    /// <summary>
    /// Get current authenticated user
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            
            return Ok(new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
    
    /// <summary>
    /// Logout user and revoke refresh token
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
            
            if (refreshToken != null && !refreshToken.IsRevoked)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }
}
```
```

---

## PROMPT 5: Backend - Configurazione e Program.cs

```
Configura il backend con JWT, CORS, Entity Framework e Swagger:

FILE: backend/LoginApi/appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=loginapp.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationAndValidation2024!",
    "Issuer": "LoginApi",
    "Audience": "LoginApiClients",
    "AccessTokenExpirationMinutes": "15",
    "RefreshTokenExpirationDays": "7"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "http://localhost:80"
    ]
  }
}
```

FILE: backend/LoginApi/appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

FILE: backend/LoginApi/Program.cs
```csharp
using LoginApi.Data;
using LoginApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and create database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

Testa il backend eseguendo:
```bash
cd backend/LoginApi
dotnet run
```
Il server sarà disponibile su http://localhost:5000
Swagger UI: http://localhost:5000/swagger
```

---

## PROMPT 6: Frontend - Setup Angular e Modelli

```
Configura il progetto Angular con gli environment e i modelli TypeScript:

FILE: frontend/src/environments/environment.ts
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

FILE: frontend/src/environments/environment.prod.ts
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://localhost:5000/api'
};
```

FILE: frontend/src/app/models/auth.model.ts
```typescript
export enum UserRole {
  Reader = 0,
  Operator = 1,
  Admin = 2
}

export interface User {
  id: number;
  username: string;
  email: string;
  role: UserRole;
  createdAt: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: Date;
  user: User;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}
```

FILE: frontend/package.json (aggiungi nella sezione scripts se non presente)
```json
{
  "scripts": {
    "start": "ng serve",
    "build": "ng build",
    "build:prod": "ng build --configuration production"
  }
}
```

Installa le dipendenze necessarie:
```bash
cd frontend
npm install
ng add @angular/material
```
```

---

## PROMPT 7: Frontend - Servizi e Interceptor

```
Crea il servizio di autenticazione e l'HTTP interceptor per gestire i token JWT:

FILE: frontend/src/app/services/auth.service.ts
```typescript
import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError, tap, catchError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import {
  User,
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  RefreshTokenRequest
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = environment.apiUrl;
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user';

  // Using signals for reactive state management
  public currentUser = signal<User | null>(null);
  public isAuthenticated = signal<boolean>(false);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadUserFromStorage();
  }

  /**
   * Load user from localStorage on service initialization
   */
  private loadUserFromStorage(): void {
    const token = this.getAccessToken();
    const userJson = localStorage.getItem(this.USER_KEY);

    if (token && userJson) {
      try {
        const user = JSON.parse(userJson);
        this.currentUser.set(user);
        this.isAuthenticated.set(true);
      } catch (error) {
        console.error('Error parsing user data:', error);
        this.clearAuthData();
      }
    }
  }

  /**
   * Register a new user
   */
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/auth/register`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response)),
        catchError(this.handleError)
      );
  }

  /**
   * Login user
   */
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/auth/login`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response)),
        catchError(this.handleError)
      );
  }

  /**
   * Refresh access token
   */
  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token available'));
    }

    const request: RefreshTokenRequest = { refreshToken };
    return this.http.post<AuthResponse>(`${this.API_URL}/auth/refresh`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response)),
        catchError(error => {
          this.logout();
          return throwError(() => error);
        })
      );
  }

  /**
   * Get current user info from API
   */
  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.API_URL}/auth/me`)
      .pipe(
        tap(user => {
          this.currentUser.set(user);
          localStorage.setItem(this.USER_KEY, JSON.stringify(user));
        }),
        catchError(this.handleError)
      );
  }

  /**
   * Logout user
   */
  logout(): void {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      const request: RefreshTokenRequest = { refreshToken };
      this.http.post(`${this.API_URL}/auth/logout`, request)
        .subscribe({
          error: (error) => console.error('Logout error:', error)
        });
    }

    this.clearAuthData();
    this.router.navigate(['/login']);
  }

  /**
   * Handle successful authentication response
   */
  private handleAuthResponse(response: AuthResponse): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));

    this.currentUser.set(response.user);
    this.isAuthenticated.set(true);
  }

  /**
   * Clear all authentication data
   */
  private clearAuthData(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);

    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  /**
   * Get access token from localStorage
   */
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  /**
   * Get refresh token from localStorage
   */
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = error.error?.message || `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    console.error('HTTP Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
```

FILE: frontend/src/app/interceptors/auth.interceptor.ts
```typescript
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

/**
 * HTTP interceptor for attaching JWT tokens and handling token refresh
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();

  // Clone request and add authorization header if token exists
  let authReq = req;
  if (token && !req.url.includes('/auth/login') && !req.url.includes('/auth/register')) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 unauthorized errors
      if (error.status === 401 && !req.url.includes('/auth/refresh')) {
        // Try to refresh the token
        return authService.refreshToken().pipe(
          switchMap(() => {
            // Retry the original request with new token
            const newToken = authService.getAccessToken();
            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newToken}`
              }
            });
            return next(retryReq);
          }),
          catchError(refreshError => {
            // If refresh fails, logout user
            authService.logout();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
```

FILE: frontend/src/app/app.config.ts (modifica per includere l'interceptor)
```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimationsAsync()
  ]
};
```
```

---

## PROMPT 8: Frontend - Guards e Routing

```
Crea i route guards per proteggere le route e configura il routing dell'applicazione:

FILE: frontend/src/app/guards/auth.guard.ts
```typescript
import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard to protect routes that require authentication
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Redirect to login page with return URL
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
```

FILE: frontend/src/app/guards/guest.guard.ts
```typescript
import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard to redirect authenticated users away from guest-only pages
 */
export const guestGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    return true;
  }

  // Redirect to dashboard if already authenticated
  router.navigate(['/dashboard']);
  return false;
};
```

FILE: frontend/src/app/app.routes.ts
```typescript
import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { 
    path: 'login', 
    component: LoginComponent,
    canActivate: [guestGuard]
  },
  { 
    path: 'register', 
    component: RegisterComponent,
    canActivate: [guestGuard]
  },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];
```
```

---

## PROMPT 9: Frontend - Componenti UI

```
Crea i componenti Angular per Login, Register, Dashboard e Navigation con Angular Material:

FILE: frontend/src/app/components/login/login.component.ts
```typescript
import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  hidePassword = signal(true);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set(null);

      const loginData: LoginRequest = this.loginForm.value;

      this.authService.login(loginData).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading.set(false);
          this.errorMessage.set(error.message || 'Login failed. Please try again.');
        }
      });
    }
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update(value => !value);
  }
}
```

FILE: frontend/src/app/components/login/login.component.html
```html
<div class="login-container">
  <mat-card class="login-card">
    <mat-card-header>
      <mat-card-title>Login</mat-card-title>
    </mat-card-header>
    <mat-card-content>
      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" placeholder="your@email.com">
          <mat-icon matPrefix>email</mat-icon>
          @if (loginForm.get('email')?.hasError('required')) {
            <mat-error>Email is required</mat-error>
          }
          @if (loginForm.get('email')?.hasError('email')) {
            <mat-error>Please enter a valid email</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Password</mat-label>
          <input matInput [type]="hidePassword() ? 'password' : 'text'" formControlName="password">
          <mat-icon matPrefix>lock</mat-icon>
          <button mat-icon-button matSuffix type="button" (click)="togglePasswordVisibility()">
            <mat-icon>{{hidePassword() ? 'visibility_off' : 'visibility'}}</mat-icon>
          </button>
          @if (loginForm.get('password')?.hasError('required')) {
            <mat-error>Password is required</mat-error>
          }
          @if (loginForm.get('password')?.hasError('minlength')) {
            <mat-error>Password must be at least 6 characters</mat-error>
          }
        </mat-form-field>

        <div class="checkbox-container">
          <mat-checkbox formControlName="rememberMe">Remember me</mat-checkbox>
        </div>

        @if (errorMessage()) {
          <div class="error-message">
            {{ errorMessage() }}
          </div>
        }

        <button mat-raised-button color="primary" type="submit" class="full-width submit-button" 
                [disabled]="loginForm.invalid || isLoading()">
          @if (isLoading()) {
            <mat-spinner diameter="20"></mat-spinner>
          } @else {
            Login
          }
        </button>
      </form>

      <div class="register-link">
        Don't have an account? <a routerLink="/register">Register here</a>
      </div>
    </mat-card-content>
  </mat-card>
</div>
```

FILE: frontend/src/app/components/login/login.component.css
```css
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  width: 100%;
  max-width: 400px;
  padding: 20px;
}

.full-width {
  width: 100%;
  margin-bottom: 16px;
}

.checkbox-container {
  margin-bottom: 16px;
}

.submit-button {
  height: 48px;
  margin-bottom: 16px;
}

.error-message {
  color: #f44336;
  margin-bottom: 16px;
  padding: 12px;
  background-color: #ffebee;
  border-radius: 4px;
  text-align: center;
}

.register-link {
  text-align: center;
  margin-top: 16px;
}

.register-link a {
  color: #3f51b5;
  text-decoration: none;
  font-weight: 500;
}

.register-link a:hover {
  text-decoration: underline;
}

mat-spinner {
  display: inline-block;
  margin: 0 auto;
}
```

Crea componenti simili per:
- Register Component (con conferma password e validazione forza password)
- Dashboard Component (mostra informazioni utente e menu)
- Navigation Component (barra di navigazione con logout)

Genera questi componenti usando:
```bash
ng generate component components/register
ng generate component components/dashboard
ng generate component components/navigation
```
```

---

## PROMPT 10: Docker e Deployment

```
Crea i file Docker per containerizzare l'applicazione:

FILE: backend/Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY LoginApi/LoginApi.csproj LoginApi/
RUN dotnet restore "LoginApi/LoginApi.csproj"

# Copy everything else and build
COPY LoginApi/ LoginApi/
WORKDIR "/src/LoginApi"
RUN dotnet build "LoginApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoginApi.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000

# Create directory for SQLite database
RUN mkdir -p /app/data

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoginApi.dll"]
```

FILE: frontend/Dockerfile
```dockerfile
# Stage 1: Build Angular application
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci

# Copy source code
COPY . .

# Build for production
RUN npm run build -- --configuration production

# Stage 2: Serve with Nginx
FROM nginx:alpine
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

FILE: frontend/nginx.conf
```nginx
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy (optional, if backend is on different domain)
    location /api {
        proxy_pass http://backend:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
}
```

FILE: docker-compose.yml
```yaml
version: '3.8'

services:
  # Backend API
  backend:
    build:
      context: ./backend
      dockerfile: Dockerfile
    container_name: login-api
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
      - ConnectionStrings__DefaultConnection=Data Source=/app/data/loginapp.db
      - JwtSettings__SecretKey=${JWT_SECRET_KEY:-YourSuperSecretKeyForJWTTokenGenerationAndValidation2024!}
      - JwtSettings__Issuer=LoginApi
      - JwtSettings__Audience=LoginApiClients
      - JwtSettings__AccessTokenExpirationMinutes=15
      - JwtSettings__RefreshTokenExpirationDays=7
      - Cors__AllowedOrigins__0=http://localhost
      - Cors__AllowedOrigins__1=http://localhost:80
    volumes:
      - backend-data:/app/data
    networks:
      - login-network
    restart: unless-stopped

  # Frontend Angular App
  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    container_name: login-frontend
    ports:
      - "80:80"
    depends_on:
      - backend
    networks:
      - login-network
    restart: unless-stopped

volumes:
  backend-data:
    driver: local

networks:
  login-network:
    driver: bridge
```

FILE: .dockerignore (nella root del progetto)
```
# Backend
backend/**/bin/
backend/**/obj/
backend/**/*.db
backend/**/*.db-shm
backend/**/*.db-wal
backend/**/Migrations/

# Frontend
frontend/node_modules/
frontend/dist/
frontend/.angular/
frontend/*.log

# General
.git/
.gitignore
README.md
*.md
.vscode/
.idea/
```

FILE: README.md
```markdown
# Full-Stack Authentication App

Application full-stack con .NET 8 Web API (backend) e Angular 18 (frontend) per l'autenticazione con JWT.

## Tecnologie

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- JWT Bearer Authentication
- BCrypt (password hashing)

### Frontend
- Angular 18 (Standalone)
- Angular Material
- TypeScript
- RxJS

## Setup Locale

### Prerequisiti
- .NET 8 SDK
- Node.js 20+
- Angular CLI 18

### Backend
```bash
cd backend/LoginApi
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm start
```

## Docker Deployment

```bash
# Avvia tutti i servizi
docker-compose up --build

# In background
docker-compose up -d --build

# Stop
docker-compose down

# Stop e rimuovi volumi
docker-compose down -v
```

### Accesso
- Frontend: http://localhost
- Backend API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

## Features

- ✅ Registrazione utenti
- ✅ Login con JWT
- ✅ Refresh token automatico
- ✅ Route guards
- ✅ HTTP interceptor
- ✅ Password hashing con BCrypt
- ✅ Validazione email e password
- ✅ User roles (Reader, Operator, Admin)
- ✅ Logout con token revocation
- ✅ Responsive UI con Material Design

## API Endpoints

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/auth/register` | POST | No | Register new user |
| `/api/auth/login` | POST | No | User login |
| `/api/auth/refresh` | POST | No | Refresh access token |
| `/api/auth/me` | GET | Yes | Get current user |
| `/api/auth/logout` | POST | Yes | Logout and revoke token |

## Sicurezza

- Password hashing con BCrypt (cost factor 12)
- JWT con scadenza (15 minuti access token, 7 giorni refresh token)
- Refresh token rotation
- CORS configurato
- Input validation
- SQL injection prevention (EF Core)
- XSS protection (Angular)
```

Per avviare l'applicazione:
```bash
# Build e avvia con Docker
docker-compose up --build

# Oppure in locale:
# Terminal 1 - Backend
cd backend/LoginApi && dotnet run

# Terminal 2 - Frontend
cd frontend && npm start
```
```

---

## 📝 NOTE FINALI

Questi 10 prompt coprono l'intera creazione dello stack:

1. **Prompt 1**: Setup iniziale della struttura
2. **Prompt 2**: Modelli del database e EF Core
3. **Prompt 3**: Servizi di autenticazione (JWT, BCrypt)
4. **Prompt 4**: Controller API e DTOs
5. **Prompt 5**: Configurazione backend (Program.cs, appsettings)
6. **Prompt 6**: Setup Angular (environment, models)
7. **Prompt 7**: Servizi frontend e interceptor
8. **Prompt 8**: Guards e routing
9. **Prompt 9**: Componenti UI con Material
10. **Prompt 10**: Docker e deployment

### Come usare questi prompt:

1. **Copia un prompt alla volta** e incollalo in GitHub Copilot
2. **Segui l'ordine** (1→10) per una costruzione logica
3. **Testa dopo ogni prompt** per verificare che tutto funzioni
4. **Personalizza** i prompt secondo le tue esigenze

### Varianti possibili:

- Sostituisci SQLite con PostgreSQL/SQL Server
- Aggiungi Redis per cache
- Implementa OAuth2/Social login
- Aggiungi email verification
- Estendi con MFA (Two-Factor Authentication)
