using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<IEnumerable<ProjectDto>> GetByPortfolioIdAsync(int portfolioId);
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(ProjectDto dto);
    Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto);
    Task<bool> DeleteAsync(int id);
}
