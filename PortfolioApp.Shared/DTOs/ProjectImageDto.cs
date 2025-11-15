namespace PortfolioApp.Shared.DTOs;

public class ProjectImageDto
{
    public int Id { get; set; }
    public string ImageBase64 { get; set; } = string.Empty;
    public int ProjectId { get; set; }
}
