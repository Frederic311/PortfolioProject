using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.Client.Services;

public interface IResumeService
{
    Task<ResumeDto?> GetByPortfolioIdAsync(int portfolioId);
    Task<ResumeDto> UploadAsync(ResumeDto dto);
    Task DeleteAsync(int id);
}
