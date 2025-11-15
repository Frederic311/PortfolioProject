using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public class SkillService : ISkillService
{
    private readonly HttpClient _httpClient;

    public SkillService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SkillDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<SkillDto>>("api/skills") ?? new();
    }

    public async Task<SkillDto?> GetByIdAsync(int id)
    {
  return await _httpClient.GetFromJsonAsync<SkillDto>($"api/skills/{id}");
    }

    public async Task<SkillDto> CreateAsync(SkillDto skill)
    {
        var response = await _httpClient.PostAsJsonAsync("api/skills", skill);
response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SkillDto>() ?? skill;
    }

 public async Task<SkillDto> UpdateAsync(int id, SkillDto skill)
    {
  var response = await _httpClient.PutAsJsonAsync($"api/skills/{id}", skill);
        response.EnsureSuccessStatusCode();
   return await response.Content.ReadFromJsonAsync<SkillDto>() ?? skill;
    }

    public async Task DeleteAsync(int id)
    {
      var response = await _httpClient.DeleteAsync($"api/skills/{id}");
        response.EnsureSuccessStatusCode();
    }
}
