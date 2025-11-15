using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.Shared.Models;

namespace PortfolioApp.API.Services;

public class SkillService : ISkillService
{
    private readonly AppDbContext _context;

    public SkillService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SkillDto>> GetAllAsync()
    {
        return await _context.Skills
            .Include(s => s.Tools)
            .OrderBy(s => s.Name)  // ✅ Ajout du tri
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<IEnumerable<SkillDto>> GetByPortfolioIdAsync(int portfolioId)
    {
        return await _context.Skills
            .Where(s => s.PortfolioId == portfolioId)
            .Include(s => s.Tools)
            .OrderBy(s => s.Name)  // ✅ Ajout du tri
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<SkillDto?> GetByIdAsync(int id)
    {
        var skill = await _context.Skills
            .Include(s => s.Tools)
            .FirstOrDefaultAsync(s => s.Id == id);

        return skill == null ? null : MapToDto(skill);
    }

    public async Task<SkillDto> CreateAsync(CreateSkillDto dto)
    {
        var skill = new Skill
        {
            Name = dto.Name,
            PortfolioId = dto.PortfolioId
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(skill.Id))!;
    }

    public async Task<SkillDto?> UpdateAsync(int id, UpdateSkillDto dto)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill == null)
            return null;

        skill.Name = dto.Name;
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var skill = await _context.Skills.FindAsync(id);

        if (skill == null)
            return false;

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return true;
    }

    private static SkillDto MapToDto(Skill skill)
    {
        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            PortfolioId = skill.PortfolioId,
            Tools = skill.Tools.Select(t => new ToolDto
            {
                Id = t.Id,
                Name = t.Name,
                SkillId = t.SkillId
            }).ToList()
        };
    }
}
