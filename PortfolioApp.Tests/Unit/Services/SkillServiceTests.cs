namespace PortfolioApp.Tests.Unit.Services;

public class SkillServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SkillService _service;

    public SkillServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;

   _context = new AppDbContext(options);
        _service = new SkillService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSkills()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
        var skills = new List<Skill>
        {
new() { Id = 1, Name = "Languages", PortfolioId = 1 },
new() { Id = 2, Name = "Frameworks", PortfolioId = 1 }
        };

    _context.Skills.AddRange(skills);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

        // Assert
    result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "Languages");
        result.Should().Contain(s => s.Name == "Frameworks");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSkill()
  {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
     _context.Portfolios.Add(portfolio);
        
 var skill = new Skill
        {
 Id = 1,
            Name = "Languages",
            PortfolioId = 1
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(1);

      // Assert
      result.Should().NotBeNull();
        result!.Name.Should().Be("Languages");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetByIdAsync(999);

     // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateSkill()
    {
     // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

        var dto = new CreateSkillDto
  {
        Name = "Databases",
            PortfolioId = 1
  };

 // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
   result.Name.Should().Be("Databases");

        var dbSkill = await _context.Skills.FindAsync(result.Id);
  dbSkill.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateSkill()
    {
      // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
_context.Portfolios.Add(portfolio);
        
        var skill = new Skill
        {
        Id = 1,
        Name = "Languages",
  PortfolioId = 1
    };

    _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        var updateDto = new UpdateSkillDto
        {
       Name = "Programming Languages"
        };

        // Act
      var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Programming Languages");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var dto = new UpdateSkillDto { Name = "Test" };

// Act
        var result = await _service.UpdateAsync(999, dto);

   // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteSkill()
    {
    // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
     _context.Portfolios.Add(portfolio);
        
        var skill = new Skill
        {
    Id = 1,
            Name = "To Delete",
            PortfolioId = 1
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
 var result = await _service.DeleteAsync(1);

  // Assert
        result.Should().BeTrue();

    var deletedSkill = await _context.Skills.FindAsync(1);
    deletedSkill.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
     var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByName()
    {
   // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
        var skills = new List<Skill>
        {
  new() { Id = 1, Name = "Zebra", PortfolioId = 1 },
        new() { Id = 2, Name = "Alpha", PortfolioId = 1 },
            new() { Id = 3, Name = "Beta", PortfolioId = 1 }
        };

        _context.Skills.AddRange(skills);
        await _context.SaveChangesAsync();

        // Act
   var result = (await _service.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alpha");
        result[1].Name.Should().Be("Beta");
    result[2].Name.Should().Be("Zebra");
    }

    public void Dispose()
    {
   _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
