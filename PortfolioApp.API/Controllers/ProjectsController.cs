using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.API.Services;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
  return Ok(projects);
    }

    // GET: api/projects/portfolio/5
    [HttpGet("portfolio/{portfolioId}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByPortfolioId(int portfolioId)
    {
        var projects = await _projectService.GetByPortfolioIdAsync(portfolioId);
    return Ok(projects);
    }

    // GET: api/projects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
  {
        var project = await _projectService.GetByIdAsync(id);
      
        if (project == null)
   return NotFound();
        
  return Ok(project);
    }

    // POST: api/projects
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectDto dto)
    {
        var project = await _projectService.CreateAsync(dto);
      return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    // PUT: api/projects/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] ProjectDto dto)
  {
   var project = await _projectService.UpdateAsync(id, dto);
        
        if (project == null)
            return NotFound();
     
        return Ok(project);
    }

    // DELETE: api/projects/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _projectService.DeleteAsync(id);
        
        if (!result)
            return NotFound();
      
        return NoContent();
 }
}
