using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public class ResumeService : IResumeService
{
    private readonly HttpClient _httpClient;

    public ResumeService(HttpClient httpClient)
    {
    _httpClient = httpClient;
    }

    public async Task<ResumeDto?> GetByPortfolioIdAsync(int portfolioId)
    {
        try
      {
        return await _httpClient.GetFromJsonAsync<ResumeDto>($"api/resumes/portfolio/{portfolioId}");
        }
        catch
        {
         return null;
        }
    }

    public async Task<ResumeDto> UploadAsync(ResumeDto dto)
    {
  var response = await _httpClient.PostAsJsonAsync("api/resumes", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ResumeDto>())!;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/resumes/{id}");
   response.EnsureSuccessStatusCode();
    }
}
