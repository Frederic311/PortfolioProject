namespace PortfolioApp.Shared.DTOs;

public class ResumeDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileContentBase64 { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public DateTime UploadedAt { get; set; }
    public int PortfolioId { get; set; }
}
