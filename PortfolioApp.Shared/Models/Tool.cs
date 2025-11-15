namespace PortfolioApp.Shared.Models;

public class Tool
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // ex: "TypeScript", "Python", "React"
    
    // Relation avec Skill
    public int SkillId { get; set; }
}
