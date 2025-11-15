using PortfolioApp.Shared.Enums;

namespace PortfolioApp.Shared.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Technologies { get; set; }
    public string? GithubUrl { get; set; }
    public string? LiveUrl { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.EnCours;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Relation avec Portfolio
    public int PortfolioId { get; set; }
    
    // Photos du projet
    public List<ProjectImage> Images { get; set; } = new();
    
    // Technologies utilisées dans ce projet
    public List<ProjectTool> ProjectTools { get; set; } = new();
}
