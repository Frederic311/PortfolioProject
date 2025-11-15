using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PortfolioApp.API.Data;
using PortfolioApp.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var isTestEnvironment = builder.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase);

// Database configuration
if (!isTestEnvironment)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Register services
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IToolService, ToolService>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<PortfolioApp.API.Services.IAuthService, PortfolioApp.API.Services.AuthService>();

// Configuration check (only in production)
if (!isTestEnvironment)
{
    Console.WriteLine("\n=== CONFIGURATION CHECK ===");
    Console.WriteLine($"Admin:Username = {builder.Configuration["Admin:Username"] ?? "NOT SET"}");
    Console.WriteLine($"Admin:PasswordHash = {(string.IsNullOrEmpty(builder.Configuration["Admin:PasswordHash"]) ? "NOT SET" : "SET")}")
;
    Console.WriteLine($"Jwt:SecretKey = {(string.IsNullOrEmpty(builder.Configuration["Jwt:SecretKey"]) ? "NOT SET" : "SET")}" );
    Console.WriteLine($"Jwt:Issuer = {builder.Configuration["Jwt:Issuer"] ?? "PortfolioApp"}");
    Console.WriteLine($"Jwt:Audience = {builder.Configuration["Jwt:Audience"] ?? "PortfolioAppClient"}");
    Console.WriteLine();
}

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:SecretKey"];

if (!isTestEnvironment && string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("Jwt:SecretKey not configured. Run: dotnet user-secrets set \"Jwt:SecretKey\" \"<your-secret>\"");
}

if (string.IsNullOrEmpty(jwtSecret))
{
    jwtSecret = "TestSecretKeyForIntegrationTestsOnly_MinimumLength32Characters";
}

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PortfolioApp",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PortfolioAppClient",
 ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
     ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS configuration
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowClient", policy =>
    {
     policy.WithOrigins(
      "https://localhost:7015",
     "http://localhost:5217",
    "https://localhost:7167",
         "http://localhost:5254",
  "http://localhost:8080",
  "https://stclient5g1wo3.z28.web.core.windows.net"
 )
   .AllowAnyHeader()
      .AllowAnyMethod()
 .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Database migrations (only in production)
if (!isTestEnvironment)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (dbContext.Database.IsSqlServer())
    {
        dbContext.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (!isTestEnvironment)
{
    Console.WriteLine("\n✅ API STARTED");
    Console.WriteLine($"HTTPS: https://localhost:7026");
    Console.WriteLine($"HTTP:  http://localhost:5158");
    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}\n");
}

app.Run();

public partial class Program { }
