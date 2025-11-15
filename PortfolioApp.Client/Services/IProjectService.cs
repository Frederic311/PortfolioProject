using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(ProjectDto project);
    Task<ProjectDto> UpdateAsync(int id, ProjectDto project);
    Task DeleteAsync(int id);
}
