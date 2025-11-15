using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.Shared.Models;

namespace PortfolioApp.API.Services;

public class ToolService : IToolService
{
    private readonly AppDbContext _context;

    public ToolService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ToolDto>> GetAllAsync()
    {
        return await _context.Tools
            .Select(t => new ToolDto
            {
                Id = t.Id,
                Name = t.Name,
                SkillId = t.SkillId
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ToolDto>> GetBySkillIdAsync(int skillId)
    {
        return await _context.Tools
            .Where(t => t.SkillId == skillId)
            .Select(t => new ToolDto
            {
                Id = t.Id,
                Name = t.Name,
                SkillId = t.SkillId
            })
            .ToListAsync();
    }

    public async Task<ToolDto?> GetByIdAsync(int id)
    {
        var tool = await _context.Tools.FindAsync(id);

        if (tool == null)
            return null;

        return new ToolDto
        {
            Id = tool.Id,
            Name = tool.Name,
            SkillId = tool.SkillId
        };
    }

    public async Task<ToolDto> CreateAsync(CreateToolDto dto)
    {
        var tool = new Tool
        {
            Name = dto.Name,
            SkillId = dto.SkillId
        };

        _context.Tools.Add(tool);
        await _context.SaveChangesAsync();

        return new ToolDto
        {
            Id = tool.Id,
            Name = tool.Name,
            SkillId = tool.SkillId
        };
    }

    public async Task<ToolDto?> UpdateAsync(int id, UpdateToolDto dto)
    {
        var tool = await _context.Tools.FindAsync(id);

        if (tool == null)
            return null;

        tool.Name = dto.Name;
        await _context.SaveChangesAsync();

        return new ToolDto
        {
            Id = tool.Id,
            Name = tool.Name,
            SkillId = tool.SkillId
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tool = await _context.Tools.FindAsync(id);

        if (tool == null)
            return false;

        _context.Tools.Remove(tool);
        await _context.SaveChangesAsync();

        return true;
    }
}
