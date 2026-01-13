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
    
    // Make first user admin if no admins exist
    if (!dbContext.Users.Any(u => u.Role == LoginApi.Models.UserRole.Admin))
    {
        var firstUser = dbContext.Users.FirstOrDefault();
        if (firstUser != null)
        {
            firstUser.Role = LoginApi.Models.UserRole.Admin;
            dbContext.SaveChanges();
            Console.WriteLine($"Set {firstUser.Username} as Admin");
        }
    }

    // Seed sample data for testing if tables are empty
    if (!dbContext.BanconoteWithdrawalAutomatic.Any())
    {
        dbContext.BanconoteWithdrawalAutomatic.AddRange(
            new LoginApi.Models.BanconoteWithdrawalAutomatic
            {
                IdStore = 371,
                StoreAlias = "Tavagnacco",
                CountingDate = DateTime.Today,
                SecurityEnvelopeCode = "ENV001",
                CountingAmount = 1200,
                CountingDifference = 0,
                WithdrawalDate = DateTime.Today.AddHours(14).AddMinutes(30),
                CountingAt = DateTime.Now,
                CountingBy = "e.albrile"
            }
        );
        dbContext.SaveChanges();
        Console.WriteLine("Added sample Cassa Intelligente data");
    }

    if (!dbContext.ExpenseFund.Any())
    {
        dbContext.ExpenseFund.AddRange(
            new LoginApi.Models.ExpenseFund
            {
                IdStore = 371,
                CountingDate = DateTime.Today,
                ExpenseType = "Forniture",
                CountingAmount = 150,
                InvoiceNumber = "F001",
                ReasonExpenses = "Acquisto materiale",
                CountingAt = DateTime.Now,
                CountingBy = "e.albrile"
            }
        );
        dbContext.SaveChanges();
        Console.WriteLine("Added sample Fondo Spese data");
    }

    if (!dbContext.CashFund.Any())
    {
        dbContext.CashFund.AddRange(
            new LoginApi.Models.CashFund
            {
                IdStore = 371,
                CashCode = "1",
                CountingDate = DateTime.Today,
                CountingAmount = 500,
                CountingAt = DateTime.Now,
                CountingBy = "e.albrile"
            },
            new LoginApi.Models.CashFund
            {
                IdStore = 371,
                CashCode = "2",
                CountingDate = DateTime.Today,
                CountingAmount = 500,
                CountingAt = DateTime.Now,
                CountingBy = "e.albrile"
            },
            new LoginApi.Models.CashFund
            {
                IdStore = 371,
                CashCode = "3",
                CountingDate = DateTime.Today,
                CountingAmount = 500,
                CountingAt = DateTime.Now,
                CountingBy = "e.albrile"
            }
        );
        dbContext.SaveChanges();
        Console.WriteLine("Added sample Fondo Cassa data");
    }

    if (!dbContext.MonetaryFund.Any())
    {
        dbContext.MonetaryFund.AddRange(
            new LoginApi.Models.MonetaryFund
            {
                IdStore = 371,
                CountingDate = DateTime.Today,
                CountingAmount = 1500,
                CountingNote = "Fondo iniziale",
                CountingAt = DateTime.Now,
                CountingBy = "e.albrile"
            }
        );
        dbContext.SaveChanges();
        Console.WriteLine("Added sample Sovv. Monetaria data");
    }
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

