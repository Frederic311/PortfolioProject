using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.Shared.Models;

namespace PortfolioApp.API.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.ProjectTools)
                .ThenInclude(pt => pt.Tool)
            .Include(p => p.Images)
                .Select(p => MapToDto(p))
                .ToListAsync();
    }

    public async Task<IEnumerable<ProjectDto>> GetByPortfolioIdAsync(int portfolioId)
    {
        return await _context.Projects
            .Where(p => p.PortfolioId == portfolioId)
            .Include(p => p.ProjectTools)
                .ThenInclude(pt => pt.Tool)
            .Include(p => p.Images)
                .Select(p => MapToDto(p))
                .ToListAsync();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectTools)
                .ThenInclude(pt => pt.Tool)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        return project == null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(ProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name ?? dto.Title ?? "",
            Title = dto.Title,
            Description = dto.Description,
            Technologies = dto.Technologies,
            GithubUrl = dto.GithubUrl,
            LiveUrl = dto.LiveUrl,
            Status = dto.Status,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            PortfolioId = dto.PortfolioId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Ajouter l'image si présente (depuis ImageUrl - format Base64)
        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            var image = new ProjectImage
            {
                ProjectId = project.Id,
                ImageBase64 = dto.ImageUrl,
                Order = 1
            };
            _context.ProjectImages.Add(image);
        }

        // Ajouter les tools sélectionnés
        if (dto.ToolIds != null && dto.ToolIds.Any())
        {
            foreach (var toolId in dto.ToolIds)
            {
                _context.ProjectTools.Add(new ProjectTool
                {
                    ProjectId = project.Id,
                    ToolId = toolId
                });
            }
        }
        
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(project.Id))!;
    }

    public async Task<ProjectDto?> UpdateAsync(int id, ProjectDto dto)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectTools)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return null;

        project.Name = dto.Name ?? dto.Title ?? "";
        project.Title = dto.Title;
        project.Description = dto.Description;
        project.Technologies = dto.Technologies;
        project.GithubUrl = dto.GithubUrl;
        project.LiveUrl = dto.LiveUrl;
        project.Status = dto.Status;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;

        // Gérer les images
        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            // Si nouvelle image, supprimer l'ancienne et ajouter la nouvelle
            var existingImages = project.Images.ToList();
            _context.ProjectImages.RemoveRange(existingImages);
            
            var newImage = new ProjectImage
            {
                ProjectId = project.Id,
                ImageBase64 = dto.ImageUrl,
                Order = 1
            };
            _context.ProjectImages.Add(newImage);
        }

        // Mettre à jour les tools
        _context.ProjectTools.RemoveRange(project.ProjectTools);
        if (dto.ToolIds != null && dto.ToolIds.Any())
        {
            foreach (var toolId in dto.ToolIds)
            {
                _context.ProjectTools.Add(new ProjectTool
                {
                    ProjectId = project.Id,
                    ToolId = toolId
                });
            }
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        
        return true;
    }

    private static ProjectDto MapToDto(Project project)
    {
        // Récupérer la première image comme ImageUrl pour compatibilité
        var firstImage = project.Images.OrderBy(i => i.Order).FirstOrDefault();
        
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Title = project.Title,
            Description = project.Description,
            Technologies = project.Technologies,
            ImageUrl = firstImage?.ImageBase64, // Première image pour compatibilité
            GithubUrl = project.GithubUrl,
            LiveUrl = project.LiveUrl,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            PortfolioId = project.PortfolioId,
            Images = project.Images.Select(i => new ProjectImageDto
            {
                Id = i.Id,
                ImageBase64 = i.ImageBase64,
                ProjectId = i.ProjectId
            }).OrderBy(i => i.Id).ToList(),
            ToolIds = project.ProjectTools.Select(pt => pt.ToolId).ToList()
        };
    }
}
