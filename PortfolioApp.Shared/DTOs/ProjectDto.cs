using PortfolioApp.Shared.Enums;

namespace PortfolioApp.Shared.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Technologies { get; set; }
    public string? ImageUrl { get; set; } // Pour la compatibilité - sera déprécié
    public string? GithubUrl { get; set; }
    public string? LiveUrl { get; set; }
    public int PortfolioId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    
    // Relations
    public List<ProjectImageDto> Images { get; set; } = new();
    public List<int> ToolIds { get; set; } = new(); // IDs des tools sélectionnés
}
