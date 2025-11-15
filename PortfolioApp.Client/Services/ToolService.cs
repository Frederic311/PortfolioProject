using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public class ToolService : IToolService
{
    private readonly HttpClient _httpClient;

    public ToolService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ToolDto>> GetAllAsync()
    {
    return await _httpClient.GetFromJsonAsync<List<ToolDto>>("api/tools") ?? new();
}

    public async Task<ToolDto?> GetByIdAsync(int id)
{
     return await _httpClient.GetFromJsonAsync<ToolDto>($"api/tools/{id}");
  }

    public async Task<ToolDto> CreateAsync(ToolDto tool)
    {
 var response = await _httpClient.PostAsJsonAsync("api/tools", tool);
     response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ToolDto>() ?? tool;
    }

    public async Task<ToolDto> UpdateAsync(int id, ToolDto tool)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tools/{id}", tool);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ToolDto>() ?? tool;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tools/{id}");
      response.EnsureSuccessStatusCode();
    }
}
