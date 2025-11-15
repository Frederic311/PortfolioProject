using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.Shared.Models;

namespace PortfolioApp.API.Services;

public class ResumeService : IResumeService
{
    private readonly AppDbContext _context;

    public ResumeService(AppDbContext context)
    {
      _context = context;
    }

    public async Task<ResumeDto?> GetByPortfolioIdAsync(int portfolioId)
    {
        var resume = await _context.Resumes
            .Where(r => r.PortfolioId == portfolioId)
            .OrderByDescending(r => r.UploadedAt)
            .FirstOrDefaultAsync();

      return resume == null ? null : MapToDto(resume);
    }

    public async Task<ResumeDto> UploadAsync(ResumeDto dto)
    {
        // Supprimer l'ancien CV s'il existe
        var existingResume = await _context.Resumes
            .Where(r => r.PortfolioId == dto.PortfolioId)
        .FirstOrDefaultAsync();

        if (existingResume != null)
   {
         _context.Resumes.Remove(existingResume);
        }

    // Ajouter le nouveau CV
        var resume = new Resume
        {
 FileName = dto.FileName,
    FileContentBase64 = dto.FileContentBase64,
  ContentType = dto.ContentType,
            PortfolioId = dto.PortfolioId,
            UploadedAt = DateTime.UtcNow
        };

   _context.Resumes.Add(resume);
  await _context.SaveChangesAsync();

        return MapToDto(resume);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var resume = await _context.Resumes.FindAsync(id);
        if (resume == null)
  return false;

      _context.Resumes.Remove(resume);
        await _context.SaveChangesAsync();

      return true;
    }

    private static ResumeDto MapToDto(Resume resume)
    {
        return new ResumeDto
        {
     Id = resume.Id,
      FileName = resume.FileName,
            FileContentBase64 = resume.FileContentBase64,
            ContentType = resume.ContentType,
    UploadedAt = resume.UploadedAt,
   PortfolioId = resume.PortfolioId
        };
    }
}
