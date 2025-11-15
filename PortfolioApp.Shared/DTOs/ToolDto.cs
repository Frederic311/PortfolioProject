namespace PortfolioApp.Shared.DTOs;

public class ToolDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SkillId { get; set; }
}

public class CreateToolDto
{
    public string Name { get; set; } = string.Empty;
    public int SkillId { get; set; }
    public int? PortfolioId { get; set; }
}

public class UpdateToolDto
{
    public string Name { get; set; } = string.Empty;
}
