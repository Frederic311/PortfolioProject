namespace PortfolioApp.Shared.DTOs;

public class PortfolioDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    
    // Profile properties
    public string? Title { get; set; }
    public string? Role { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImage { get; set; }  // Base64 image
 public string? ContactEmail { get; set; }
    public string? LinkedIn { get; set; }
    public string? Github { get; set; }
}

public class CreatePortfolioDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Role { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImage { get; set; }  // Base64 image
    public string? ContactEmail { get; set; }
    public string? LinkedIn { get; set; }
    public string? Github { get; set; }
}

public class UpdatePortfolioDto
{
  public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
public string? Title { get; set; }
    public string? Role { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImage { get; set; }  // Base64 image
    public string? ContactEmail { get; set; }
  public string? LinkedIn { get; set; }
    public string? Github { get; set; }
}
