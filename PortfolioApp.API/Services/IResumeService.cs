using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Services;

public interface IResumeService
{
    Task<ResumeDto?> GetByPortfolioIdAsync(int portfolioId);
    Task<ResumeDto> UploadAsync(ResumeDto dto);
    Task<bool> DeleteAsync(int id);
}
