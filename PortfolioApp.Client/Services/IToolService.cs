using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public interface IToolService
{
    Task<List<ToolDto>> GetAllAsync();
    Task<ToolDto?> GetByIdAsync(int id);
    Task<ToolDto> CreateAsync(ToolDto tool);
    Task<ToolDto> UpdateAsync(int id, ToolDto tool);
    Task DeleteAsync(int id);
}
