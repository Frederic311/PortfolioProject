using System.Net.Http.Json;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public class ProjectService : IProjectService
{
    private readonly HttpClient _httpClient;

    public ProjectService(HttpClient httpClient)
    {
_httpClient = httpClient;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? new();
  }

    public async Task<ProjectDto?> GetByIdAsync(int id)
 {
    return await _httpClient.GetFromJsonAsync<ProjectDto>($"api/projects/{id}");
    }

    public async Task<ProjectDto> CreateAsync(ProjectDto project)
    {
        var response = await _httpClient.PostAsJsonAsync("api/projects", project);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDto>() ?? project;
    }

    public async Task<ProjectDto> UpdateAsync(int id, ProjectDto project)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/projects/{id}", project);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDto>() ?? project;
    }

    public async Task DeleteAsync(int id)
  {
    var response = await _httpClient.DeleteAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }
}
