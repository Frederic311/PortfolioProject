using PortfolioApp.Shared.Enums;

namespace PortfolioApp.Tests.Unit.Services;

public class ProjectServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        // Créer une base de données InMemory unique pour chaque test
        var options = new DbContextOptionsBuilder<AppDbContext>()
  .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
  _service = new ProjectService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProjects()
    {
 // Arrange
    var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
 var projects = new List<Project>
        {
     new() { Id = 1, Name = "Project 1", Title = "Project 1", Description = "Desc 1", PortfolioId = 1 },
   new() { Id = 2, Name = "Project 2", Title = "Project 2", Description = "Desc 2", PortfolioId = 1 }
        };
   
        _context.Projects.AddRange(projects);
  await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Project 1");
        result.Should().Contain(p => p.Name == "Project 2");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProject()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
     
   var project = new Project
        {
    Id = 1,
      Name = "Test Project",
            Title = "Test Title",
Description = "Test Description",
            Technologies = "C#, .NET",
       PortfolioId = 1
        };

    _context.Projects.Add(project);
  await _context.SaveChangesAsync();

        // Act
     var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Project");
      result.Title.Should().Be("Test Title");
   result.Description.Should().Be("Test Description");
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
    public async Task CreateAsync_WithValidData_ShouldCreateProject()
    {
    // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

        var dto = new ProjectDto
   {
            Name = "New Project",
      Title = "New Project Title",
   Description = "Test Description",
        Technologies = "React, Node.js",
            GithubUrl = "https://github.com/test",
            LiveUrl = "https://test.com",
 Status = ProjectStatus.EnCours,
            PortfolioId = 1
        };

   // Act
        var result = await _service.CreateAsync(dto);

      // Assert
    result.Should().NotBeNull();
      result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Project");
        result.Title.Should().Be("New Project Title");
  
        // Vérifier que le projet est bien dans la base
        var dbProject = await _context.Projects.FindAsync(result.Id);
    dbProject.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_WithImageBase64_ShouldCreateProjectWithImage()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
    _context.Portfolios.Add(portfolio);
      await _context.SaveChangesAsync();

      var dto = new ProjectDto
     {
 Name = "Project With Image",
            Title = "Project With Image",
     Description = "Test",
         ImageUrl = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
            Status = ProjectStatus.EnCours,
 PortfolioId = 1
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
   result.Images.Should().HaveCount(1);
        result.Images.First().ImageBase64.Should().NotBeNullOrEmpty();
    }

[Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProject()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);
        
        var project = new Project
      {
    Id = 1,
            Name = "Original Name",
         Title = "Original Title",
     Description = "Original Description",
        PortfolioId = 1
        };
  
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

   var updateDto = new ProjectDto
        {
   Name = "Updated Name",
            Title = "Updated Title",
        Description = "Updated Description",
 Technologies = "Updated Tech",
      Status = ProjectStatus.Termine,
  PortfolioId = 1
        };

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

   // Assert
        result.Should().NotBeNull();
result!.Name.Should().Be("Updated Name");
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Status.Should().Be(ProjectStatus.Termine);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var dto = new ProjectDto { Name = "Test", Title = "Test", PortfolioId = 1 };

        // Act
        var result = await _service.UpdateAsync(999, dto);

        // Assert
 result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteProject()
    {
        // Arrange
        var portfolio = new Portfolio { Id = 1, Name = "Test Portfolio" };
        _context.Portfolios.Add(portfolio);

var project = new Project
        {
            Id = 1,
      Name = "To Delete",
       Title = "To Delete",
            Description = "Test",
     PortfolioId = 1
        };
   
        _context.Projects.Add(project);
await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
 
        var deletedProject = await _context.Projects.FindAsync(1);
      deletedProject.Should().BeNull();
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
    public async Task GetByPortfolioIdAsync_ShouldReturnOnlyProjectsForThatPortfolio()
    {
        // Arrange
        var portfolio1 = new Portfolio { Id = 1, Name = "Portfolio 1" };
   var portfolio2 = new Portfolio { Id = 2, Name = "Portfolio 2" };
        _context.Portfolios.AddRange(portfolio1, portfolio2);
  
        var projects = new List<Project>
        {
            new() { Id = 1, Name = "P1 Project 1", Title = "P1 Project 1", Description = "Test", PortfolioId = 1 },
            new() { Id = 2, Name = "P1 Project 2", Title = "P1 Project 2", Description = "Test", PortfolioId = 1 },
         new() { Id = 3, Name = "P2 Project 1", Title = "P2 Project 1", Description = "Test", PortfolioId = 2 }
    };
      
        _context.Projects.AddRange(projects);
   await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByPortfolioIdAsync(1);

        // Assert
      result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.PortfolioId.Should().Be(1));
    }

    public void Dispose()
    {
     _context.Database.EnsureDeleted();
        _context.Dispose();
}
}
