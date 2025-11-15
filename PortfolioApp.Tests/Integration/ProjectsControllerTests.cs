using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.Models;
using PortfolioApp.Shared.DTOs;
using FluentAssertions;
using Xunit;

namespace PortfolioApp.Tests.Integration;

public class ProjectsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly AppDbContext _context;

    public ProjectsControllerTests(WebApplicationFactory<Program> factory)
    {
  var dbName = $"TestDb_{Guid.NewGuid()}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
   builder.UseEnvironment("Testing");
            
     builder.ConfigureServices(services =>
   {
    // Remove existing DbContext to avoid conflicts
    var descriptors = services
       .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
    d.ServiceType == typeof(AppDbContext))
 .ToList();

    foreach (var descriptor in descriptors)
           {
 services.Remove(descriptor);
            }

 // Add InMemory database for tests
      services.AddDbContext<AppDbContext>(options =>
   {
        options.UseInMemoryDatabase(dbName);
 });
         });
});

_client = _factory.CreateClient();
        
  var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
   SeedDatabase();
    }

    private void SeedDatabase()
    {
    var portfolio = new Portfolio
        {
        Id = 1,
  Name = "Test Portfolio",
            Description = "Test"
     };

        _context.Portfolios.Add(portfolio);

        var projects = new List<Project>
 {
            new() { Id = 1, Name = "Project 1", Title = "Project 1", Description = "Test 1", PortfolioId = 1 },
         new() { Id = 2, Name = "Project 2", Title = "Project 2", Description = "Test 2", PortfolioId = 1 }
    };

      _context.Projects.AddRange(projects);
        _context.SaveChanges();
    }

  [Fact]
    public async Task GetAll_ShouldReturn200_WithProjects()
    {
      // Act
        var response = await _client.GetAsync("/api/projects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
     
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
   projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThan(0);
 }

    [Fact]
 public async Task GetById_WithValidId_ShouldReturn200()
    {
   // Act
        var response = await _client.GetAsync("/api/projects/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
   
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        project.Should().NotBeNull();
        project!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/api/projects/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

  [Fact]
    public async Task GetByPortfolioId_ShouldReturnProjectsForPortfolio()
    {
   // Act
        var response = await _client.GetAsync("/api/projects/portfolio/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        projects.Should().NotBeNull();
        projects.Should().AllSatisfy(p => p.PortfolioId.Should().Be(1));
    }

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
        _context.Dispose();
        _client.Dispose();
    }
}
