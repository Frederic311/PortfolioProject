namespace PortfolioApp.Tests.Unit.Services;

public class ToolServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ToolService _service;

    public ToolServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
  .Options;

 _context = new AppDbContext(options);
        _service = new ToolService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTools()
 {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
        var skill = new Skill { Id = 1, Name = "Languages", PortfolioId = 1 };
    _context.Skills.Add(skill);
        
        var tools = new List<Tool>
        {
     new() { Id = 1, Name = "C#", SkillId = 1 },
   new() { Id = 2, Name = "Python", SkillId = 1 }
  };

        _context.Tools.AddRange(tools);
        await _context.SaveChangesAsync();

   // Act
  var result = await _service.GetAllAsync();

// Assert
        result.Should().HaveCount(2);
  result.Should().Contain(t => t.Name == "C#");
 result.Should().Contain(t => t.Name == "Python");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTool()
{
        // Arrange
 var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
 var skill = new Skill { Id = 1, Name = "Languages", PortfolioId = 1 };
 _context.Skills.Add(skill);
        
        var tool = new Tool
   {
            Id = 1,
  Name = "TypeScript",
  SkillId = 1
        };

        _context.Tools.Add(tool);
        await _context.SaveChangesAsync();

     // Act
        var result = await _service.GetByIdAsync(1);

    // Assert
        result.Should().NotBeNull();
  result!.Name.Should().Be("TypeScript");
    }

    [Fact]
  public async Task CreateAsync_WithValidData_ShouldCreateTool()
    {
  // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
 var skill = new Skill { Id = 1, Name = "Languages", PortfolioId = 1 };
   _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        var dto = new CreateToolDto
        {
    Name = "JavaScript",
  SkillId = 1,
     PortfolioId = 1
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
     result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
  result.Name.Should().Be("JavaScript");

        var dbTool = await _context.Tools.FindAsync(result.Id);
    dbTool.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateTool()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
   _context.Portfolios.Add(portfolio);
        
        var skill = new Skill { Id = 1, Name = "Languages", PortfolioId = 1 };
        _context.Skills.Add(skill);
        
    var tool = new Tool
  {
      Id = 1,
   Name = "Java",
 SkillId = 1
  };

        _context.Tools.Add(tool);
    await _context.SaveChangesAsync();

        var updateDto = new UpdateToolDto
     {
Name = "Java 21"
        };

      // Act
      var result = await _service.UpdateAsync(1, updateDto);

     // Assert
     result.Should().NotBeNull();
        result!.Name.Should().Be("Java 21");
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteTool()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
     _context.Portfolios.Add(portfolio);
     
        var skill = new Skill { Id = 1, Name = "Languages", PortfolioId = 1 };
        _context.Skills.Add(skill);
        
        var tool = new Tool
        {
          Id = 1,
            Name = "To Delete",
       SkillId = 1
        };

  _context.Tools.Add(tool);
        await _context.SaveChangesAsync();

    // Act
        var result = await _service.DeleteAsync(1);

    // Assert
     result.Should().BeTrue();

      var deletedTool = await _context.Tools.FindAsync(1);
        deletedTool.Should().BeNull();
    }

 public void Dispose()
    {
    _context.Database.EnsureDeleted();
   _context.Dispose();
 }
}
