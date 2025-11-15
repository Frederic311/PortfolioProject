namespace PortfolioApp.Tests.Unit.Services;

public class PortfolioServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PortfolioService _service;

    public PortfolioServiceTests()
    {
    var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
   .Options;

     _context = new AppDbContext(options);
        _service = new PortfolioService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPortfolios()
    {
        // Arrange
        var portfolios = new List<Portfolio>
        {
   new() { Id = 1, Name = "Portfolio 1", Description = "Desc 1" },
       new() { Id = 2, Name = "Portfolio 2", Description = "Desc 2" }
        };

        _context.Portfolios.AddRange(portfolios);
        await _context.SaveChangesAsync();

        // Act
   var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
result.Should().Contain(p => p.Name == "Portfolio 1");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPortfolio()
    {
        // Arrange
      var portfolio = new Portfolio
        {
         Id = 1,
            Name = "Test Portfolio",
       Description = "Test Description",
      Bio = "Test Bio"
        };

        _context.Portfolios.Add(portfolio);
    await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Portfolio");
        result.Bio.Should().Be("Test Bio");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreatePortfolio()
 {
        // Arrange
        var dto = new CreatePortfolioDto
   {
        Name = "New Portfolio",
         Description = "New Description",
Bio = "New Bio",
         ContactEmail = "test@example.com"
   };

// Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
      result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Portfolio");
  result.ContactEmail.Should().Be("test@example.com");

        var dbPortfolio = await _context.Portfolios.FindAsync(result.Id);
        dbPortfolio.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdatePortfolio()
  {
        // Arrange
        var portfolio = new Portfolio
     {
            Id = 1,
            Name = "Original",
        Description = "Original Desc"
        };

  _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

        var updateDto = new UpdatePortfolioDto
        {
   Name = "Updated",
            Description = "Updated Desc",
    Bio = "Updated Bio"
        };

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
      result.Should().NotBeNull();
      result!.Name.Should().Be("Updated");
  result.Description.Should().Be("Updated Desc");
  result.Bio.Should().Be("Updated Bio");
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeletePortfolio()
    {
        // Arrange
        var portfolio = new Portfolio
        {
            Id = 1,
     Name = "To Delete",
  Description = "Test"
        };

    _context.Portfolios.Add(portfolio);
   await _context.SaveChangesAsync();

        // Act
     var result = await _service.DeleteAsync(1);

   // Assert
        result.Should().BeTrue();

        var deletedPortfolio = await _context.Portfolios.FindAsync(1);
    deletedPortfolio.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPortfolio()
    {
        // Arrange
   var portfolio = new Portfolio
        {
     Id = 1,
    Name = "Portfolio Test",
     Description = "Test"
  };

     _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

    // Act
        var result = await _service.GetByIdAsync(1);

     // Assert
  result.Should().NotBeNull();
        result!.Name.Should().Be("Portfolio Test");
    }

    public void Dispose()
  {
 _context.Database.EnsureDeleted();
 _context.Dispose();
    }
}
