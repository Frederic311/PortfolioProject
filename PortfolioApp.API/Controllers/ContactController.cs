using Microsoft.AspNetCore.Mvc;
using PortfolioApp.API.Services;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IPortfolioService _portfolioService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
   IEmailService emailService,
  IPortfolioService portfolioService,
        ILogger<ContactController> logger)
    {
   _emailService = emailService;
        _portfolioService = portfolioService;
 _logger = logger;
    }

    // POST: api/contact
    [HttpPost]
    public async Task<IActionResult> SendContactMessage([FromBody] ContactMessageDto message)
    {
        try
   {
      // Récupérer le portfolio pour obtenir l'email de contact
      var portfolios = await _portfolioService.GetAllAsync();
     var portfolio = portfolios.FirstOrDefault(p => p.Id == message.PortfolioId);

     if (portfolio == null || string.IsNullOrEmpty(portfolio.ContactEmail))
   {
      _logger.LogWarning($"Portfolio {message.PortfolioId} not found or no contact email configured");
          return BadRequest(new { message = "Contact email not configured" });
  }

   // Envoyer l'email au propriétaire du portfolio
 await _emailService.SendContactEmailAsync(message, portfolio.ContactEmail);

      return Ok(new { message = "Message sent successfully" });
        }
        catch (Exception ex)
     {
      _logger.LogError(ex, "Error sending contact message");
      return StatusCode(500, new { message = "Failed to send message. Please try again later." });
        }
    }
}
