using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public interface ISkillService
{
    Task<List<SkillDto>> GetAllAsync();
    Task<SkillDto?> GetByIdAsync(int id);
    Task<SkillDto> CreateAsync(SkillDto skill);
    Task<SkillDto> UpdateAsync(int id, SkillDto skill);
    Task DeleteAsync(int id);
}
