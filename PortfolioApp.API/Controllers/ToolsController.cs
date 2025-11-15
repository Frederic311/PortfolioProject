using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.API.Services;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    private readonly IToolService _toolService;

    public ToolsController(IToolService toolService)
    {
        _toolService = toolService;
    }

    // GET: api/tools
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToolDto>>> GetAll()
    {
        var tools = await _toolService.GetAllAsync();
        return Ok(tools);
    }

    // GET: api/tools/skill/5
    [HttpGet("skill/{skillId}")]
    public async Task<ActionResult<IEnumerable<ToolDto>>> GetBySkillId(int skillId)
    {
        var tools = await _toolService.GetBySkillIdAsync(skillId);
        return Ok(tools);
    }

    // GET: api/tools/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ToolDto>> GetById(int id)
    {
        var tool = await _toolService.GetByIdAsync(id);
        
        if (tool == null)
            return NotFound();

        return Ok(tool);
    }

    // POST: api/tools
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ToolDto>> Create(CreateToolDto dto)
    {
        var tool = await _toolService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tool.Id }, tool);
    }

    // PUT: api/tools/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ToolDto>> Update(int id, UpdateToolDto dto)
    {
        var tool = await _toolService.UpdateAsync(id, dto);
        
        if (tool == null)
            return NotFound();

        return Ok(tool);
    }

    // DELETE: api/tools/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _toolService.DeleteAsync(id);
        
        if (!result)
            return NotFound();

        return NoContent();
    }
}
