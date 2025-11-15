using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
 {
   _configuration = configuration;
      _logger = logger;
    }

    public async Task SendContactEmailAsync(ContactMessageDto message, string recipientEmail)
    {
     try
        {
  var senderEmail = _configuration["Email:SenderEmail"] ?? throw new InvalidOperationException("Email:SenderEmail not configured");
      var appPassword = _configuration["Email:AppPassword"] ?? throw new InvalidOperationException("Email:AppPassword not configured");

      using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
      {
EnableSsl = true,
     Credentials = new NetworkCredential(senderEmail, appPassword)
            };

            var mailMessage = new MailMessage
    {
          From = new MailAddress(senderEmail, "Portfolio Contact Form"),
     Subject = $"New contact from {message.Name}",
          Body = $@"
<html>
<body>
    <h2>New Contact Message</h2>
    <p><strong>From:</strong> {message.Name}</p>
<p><strong>Email:</strong> {message.Email}</p>
    <p><strong>Message:</strong></p>
    <p>{message.Message}</p>
</body>
</html>",
                IsBodyHtml = true
  };

       mailMessage.To.Add(recipientEmail);
       mailMessage.ReplyToList.Add(new MailAddress(message.Email, message.Name));

        await smtpClient.SendMailAsync(mailMessage);
      _logger.LogInformation("Email sent successfully to {Recipient}", recipientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }
}
