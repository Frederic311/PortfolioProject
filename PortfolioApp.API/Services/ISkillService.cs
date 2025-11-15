using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllAsync();
    Task<IEnumerable<SkillDto>> GetByPortfolioIdAsync(int portfolioId);
    Task<SkillDto?> GetByIdAsync(int id);
    Task<SkillDto> CreateAsync(CreateSkillDto dto);
    Task<SkillDto?> UpdateAsync(int id, UpdateSkillDto dto);
    Task<bool> DeleteAsync(int id);
}
