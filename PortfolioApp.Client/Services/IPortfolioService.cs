using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public interface IPortfolioService
{
    Task<List<PortfolioDto>> GetAllAsync();
    Task<PortfolioDto?> GetByIdAsync(int id);
    Task<PortfolioDto> CreateAsync(PortfolioDto portfolio);
    Task<PortfolioDto> UpdateAsync(int id, PortfolioDto portfolio);
    Task DeleteAsync(int id);
}
