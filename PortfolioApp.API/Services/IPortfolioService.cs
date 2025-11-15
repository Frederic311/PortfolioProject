using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public interface IPortfolioService
{
    Task<IEnumerable<PortfolioDto>> GetAllAsync();
    Task<PortfolioDto?> GetByIdAsync(int id);
    Task<PortfolioDto> CreateAsync(CreatePortfolioDto dto);
    Task<PortfolioDto?> UpdateAsync(int id, UpdatePortfolioDto dto);
    Task<bool> DeleteAsync(int id);
}
