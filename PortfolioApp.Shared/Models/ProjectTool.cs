namespace PortfolioApp.Shared.Models;

public class ProjectTool
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int ToolId { get; set; }
    
    // Navigation properties
    public Project Project { get; set; } = null!;
    public Tool Tool { get; set; } = null!;
}
