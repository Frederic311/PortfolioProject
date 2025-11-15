namespace PortfolioApp.Shared.DTOs;

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PortfolioId { get; set; }
    public List<ToolDto> Tools { get; set; } = new();
}

public class CreateSkillDto
{
    public string Name { get; set; } = string.Empty;
    public int PortfolioId { get; set; }
}

public class UpdateSkillDto
{
    public string Name { get; set; } = string.Empty;
}
