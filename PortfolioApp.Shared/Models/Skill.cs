namespace PortfolioApp.Shared.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // ex: "Languages", "Frameworks", "Databases"
    
    // Relation avec Portfolio
    public int PortfolioId { get; set; }
    
    // Relation avec Tools
    public List<Tool> Tools { get; set; } = new();
}
