namespace PortfolioApp.Shared.Models;

public class ProjectImage
{
    public int Id { get; set; }
    public string ImageBase64 { get; set; } = string.Empty;
    public string? Caption { get; set; } // Description de l'image
    public int Order { get; set; } // Pour trier les images
    
    // Relation avec Project
    public int ProjectId { get; set; }
}
