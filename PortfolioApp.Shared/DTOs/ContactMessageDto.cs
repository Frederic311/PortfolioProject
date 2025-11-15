namespace PortfolioApp.Shared.DTOs;

public class ContactMessageDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int PortfolioId { get; set; }
}
