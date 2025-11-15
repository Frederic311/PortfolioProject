using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public class PortfolioService : IPortfolioService
{
    private readonly HttpClient _httpClient;

    public PortfolioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PortfolioDto>> GetAllAsync()
    {
  return await _httpClient.GetFromJsonAsync<List<PortfolioDto>>("api/portfolios") ?? new();
}

    public async Task<PortfolioDto?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<PortfolioDto>($"api/portfolios/{id}");
  }

    public async Task<PortfolioDto> CreateAsync(PortfolioDto portfolio)
    {
        var response = await _httpClient.PostAsJsonAsync("api/portfolios", portfolio);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PortfolioDto>() ?? portfolio;
    }

    public async Task<PortfolioDto> UpdateAsync(int id, PortfolioDto portfolio)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/portfolios/{id}", portfolio);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PortfolioDto>() ?? portfolio;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/portfolios/{id}");
        response.EnsureSuccessStatusCode();
 }
}
