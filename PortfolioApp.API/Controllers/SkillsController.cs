using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.API.Services;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    // GET: api/skills
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetAll()
    {
        var skills = await _skillService.GetAllAsync();
        return Ok(skills);
    }

    // GET: api/skills/portfolio/5
    [HttpGet("portfolio/{portfolioId}")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetByPortfolioId(int portfolioId)
    {
        var skills = await _skillService.GetByPortfolioIdAsync(portfolioId);
        return Ok(skills);
    }

    // GET: api/skills/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SkillDto>> GetById(int id)
    {
        var skill = await _skillService.GetByIdAsync(id);
        
        if (skill == null)
            return NotFound();

        return Ok(skill);
    }

    // POST: api/skills
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SkillDto>> Create(CreateSkillDto dto)
    {
        var skill = await _skillService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = skill.Id }, skill);
    }

    // PUT: api/skills/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<SkillDto>> Update(int id, UpdateSkillDto dto)
    {
        var skill = await _skillService.UpdateAsync(id, dto);
        
        if (skill == null)
            return NotFound();

        return Ok(skill);
    }

    // DELETE: api/skills/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _skillService.DeleteAsync(id);
        
        if (!result)
            return NotFound();

        return NoContent();
    }
}
