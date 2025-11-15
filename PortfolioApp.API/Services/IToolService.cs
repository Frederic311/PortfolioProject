using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public interface IToolService
{
    Task<IEnumerable<ToolDto>> GetAllAsync();
    Task<IEnumerable<ToolDto>> GetBySkillIdAsync(int skillId);
    Task<ToolDto?> GetByIdAsync(int id);
    Task<ToolDto> CreateAsync(CreateToolDto dto);
    Task<ToolDto?> UpdateAsync(int id, UpdateToolDto dto);
    Task<bool> DeleteAsync(int id);
}
