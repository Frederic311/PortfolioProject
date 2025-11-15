using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public interface IContactService
{
    Task SendContactMessageAsync(ContactMessageDto message);
}
