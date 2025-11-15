using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioApp.API.Services;

public class AuthService : IAuthService
{
 private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly PasswordHasher<string> _passwordHasher;

    public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
    {
     _configuration = configuration;
        _logger = logger;
        _passwordHasher = new PasswordHasher<string>();
    }

    public bool ValidateCredentials(string username, string password)
    {
  var adminUsername = _configuration["Admin:Username"];
        var adminPasswordHash = _configuration["Admin:PasswordHash"];

        if (string.IsNullOrEmpty(adminUsername) || string.IsNullOrEmpty(adminPasswordHash))
        {
            _logger.LogError("Admin credentials not configured");
   throw new InvalidOperationException("Admin credentials not configured");
        }

        if (!username.Equals(adminUsername, StringComparison.Ordinal))
      {
  return false;
        }

 var result = _passwordHasher.VerifyHashedPassword(username, adminPasswordHash, password);

        if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
   {
            return true;
  }

    return false;
    }

    public string GenerateJwtToken(string username)
    {
        var jwtSecret = _configuration["Jwt:SecretKey"];
    if (string.IsNullOrEmpty(jwtSecret))
   {
      _logger.LogError("JWT SecretKey not configured");
   throw new InvalidOperationException("Jwt:SecretKey not configured");
        }

        var issuer = _configuration["Jwt:Issuer"] ?? "PortfolioApp";
        var audience = _configuration["Jwt:Audience"] ?? "PortfolioAppClient";
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "120");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

 var claims = new[]
        {
   new Claim(JwtRegisteredClaimNames.Sub, username),
     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
     new Claim(ClaimTypes.Name, username),
       new Claim(ClaimTypes.Role, "Admin")
        };

     var token = new JwtSecurityToken(
    issuer: issuer,
   audience: audience,
       claims: claims,
       expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
 signingCredentials: credentials
        );

  return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
