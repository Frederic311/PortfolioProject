using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public interface IEmailService
{
    Task SendContactEmailAsync(ContactMessageDto message, string recipientEmail);
}
