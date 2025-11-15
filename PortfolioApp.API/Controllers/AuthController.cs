using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.API.Services;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto dto)
    {
        if (dto is null)
        {
        return BadRequest(new { message = "Invalid request" });
        }

        try
        {
            var isValid = _authService.ValidateCredentials(dto.Username, dto.Password);

            if (isValid)
      {
         var token = _authService.GenerateJwtToken(dto.Username);
    var expiresAt = DateTime.UtcNow.AddMinutes(120);

     _logger.LogInformation("Login successful for user: {Username}", dto.Username);

     return Ok(new LoginResponseDto
    {
Token = token,
       ExpiresAt = expiresAt
     });
        }
  else
         {
           _logger.LogWarning("Invalid credentials for username: {Username}", dto.Username);
    return Unauthorized(new { message = "Invalid username or password" });
            }
  }
        catch (InvalidOperationException ex)
    {
 _logger.LogError(ex, "Configuration error during login");
return StatusCode(500, new { message = "Server configuration error. Check logs." });
        }
  catch (Exception ex)
        {
       _logger.LogError(ex, "Unexpected error during login");
   return StatusCode(500, new { message = "An error occurred during login" });
     }
    }
}
