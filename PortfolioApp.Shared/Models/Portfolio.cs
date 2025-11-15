using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Shared.Models;

public class Portfolio
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Profile properties
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(200)]
    public string? Role { get; set; }  // Nouveau champ

    public string? Bio { get; set; }
    
    public string? ProfileImage { get; set; }  // Base64 image
    
    [EmailAddress]
    [MaxLength(200)]
    public string? ContactEmail { get; set; }
 
    [MaxLength(300)]
    public string? LinkedIn { get; set; }
    
    [MaxLength(300)]
    public string? Github { get; set; }

    // Relations
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public Resume? Resume { get; set; }
}
