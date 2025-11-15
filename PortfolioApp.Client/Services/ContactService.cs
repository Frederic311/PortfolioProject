using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendContactMessageAsync(ContactMessageDto message)
    {
  var response = await _httpClient.PostAsJsonAsync("api/contact", message);
    response.EnsureSuccessStatusCode();
    }
}
