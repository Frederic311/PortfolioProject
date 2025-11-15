using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.Models;
using PortfolioApp.Shared.DTOs;
using FluentAssertions;
using Xunit;
using PortfolioApp.Tests.Helpers;

namespace PortfolioApp.Tests.Integration.Authenticated;

public class AuthenticatedProjectsTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
  private HttpClient _client;
    private readonly AppDbContext _context;

    public AuthenticatedProjectsTests(WebApplicationFactory<Program> factory)
    {
  var dbName = $"AuthTestDb_{Guid.NewGuid()}";
        _factory = factory.WithWebHostBuilder(builder =>
        {
      builder.UseEnvironment("Testing");
            
            builder.ConfigureAppConfiguration((context, config) =>
            {
        config.AddInMemoryCollection(new Dictionary<string, string>
            {
     ["Jwt:SecretKey"] = "TestSecretKeyForIntegrationTestsOnly_MinimumLength32Characters",
        ["Jwt:Issuer"] = "PortfolioApp",
            ["Jwt:Audience"] = "PortfolioAppClient",
         ["Jwt:ExpiryMinutes"] = "120",
            ["Admin:Username"] = "admin",
        ["Admin:PasswordHash"] = "AQAAAAIAAYagAAAAELIv6IcP3IyQ8A1IOZYPylQ32Sd6dnE/6446+/xDXqszkBhN5+Gn/rWjr2/6JRgyIQ=="
  }!);
      });

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
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateProject_WithAuthentication_ShouldReturn201()
    {
    // Arrange
        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var authHelper = new AuthHelper(_client, config);
        _client = await authHelper.GetAuthenticatedClientAsync();

        var newProject = new ProjectDto
    {
         Name = "New Project",
            Title = "New Project",
      Description = "Integration Test Project",
   Technologies = "C#, xUnit",
            PortfolioId = 1
     };

        // Act
   var response = await _client.PostAsJsonAsync("/api/projects", newProject);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdProject = await response.Content.ReadFromJsonAsync<ProjectDto>();
        createdProject.Should().NotBeNull();
        createdProject!.Name.Should().Be("New Project");
    }

    [Fact]
    public async Task UpdateProject_WithAuthentication_ShouldReturn200()
    {
  // Arrange
        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var authHelper = new AuthHelper(_client, config);
   _client = await authHelper.GetAuthenticatedClientAsync();

  var createDto = new ProjectDto
        {
 Name = "Original",
 Title = "Original",
            Description = "Original",
  PortfolioId = 1
        };

 var createResponse = await _client.PostAsJsonAsync("/api/projects", createDto);
     createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
var created = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

   // Act
        var updateDto = new ProjectDto
 {
        Name = "Updated",
      Title = "Updated",
 Description = "Updated Description",
            PortfolioId = 1
   };

    var response = await _client.PutAsJsonAsync($"/api/projects/{created!.Id}", updateDto);

        // Assert
      response.StatusCode.Should().Be(HttpStatusCode.OK);
        
   if (response.Content.Headers.ContentLength > 0)
     {
  var updated = await response.Content.ReadFromJsonAsync<ProjectDto>();
 updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated");
            updated.Description.Should().Be("Updated Description");
        }
    }

    [Fact]
    public async Task DeleteProject_WithAuthentication_ShouldReturn204()
    {
        // Arrange - Create a project first
        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var authHelper = new AuthHelper(_client, config);
        _client = await authHelper.GetAuthenticatedClientAsync();

        var createDto = new ProjectDto
   {
       Name = "To Delete",
  Title = "To Delete",
            Description = "Test",
            PortfolioId = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/projects", createDto);
  createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/projects/{created!.Id}");

        // Assert
 response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
    // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/projects/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
 }
    [Fact]
    public async Task CreateProject_WithoutAuthentication_ShouldReturn401()
    {
        // Arrange - Create new client without auth
        var unauthClient = _factory.CreateClient();

        var newProject = new ProjectDto
        {
            Name = "Test",
          Title = "Test",
        Description = "Test",
  PortfolioId = 1
        };

        // Act
        var response = await unauthClient.PostAsJsonAsync("/api/projects", newProject);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public void Dispose()
    {
     _context.Database.EnsureDeleted();
   _context.Dispose();
        _client.Dispose();
    }
}
