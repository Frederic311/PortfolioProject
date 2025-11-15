using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.API.Services;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;

    public PortfoliosController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    // GET: api/portfolios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetAll()
    {
        var portfolios = await _portfolioService.GetAllAsync();
        return Ok(portfolios);
    }

    // GET: api/portfolios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioDto>> GetById(int id)
    {
        var portfolio = await _portfolioService.GetByIdAsync(id);
        
        if (portfolio == null)
            return NotFound();

        return Ok(portfolio);
    }

    // POST: api/portfolios
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PortfolioDto>> Create(CreatePortfolioDto dto)
    {
        var portfolio = await _portfolioService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = portfolio.Id }, portfolio);
    }

    // PUT: api/portfolios/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<PortfolioDto>> Update(int id, UpdatePortfolioDto dto)
    {
        var portfolio = await _portfolioService.UpdateAsync(id, dto);
        
        if (portfolio == null)
            return NotFound();

        return Ok(portfolio);
    }

    // DELETE: api/portfolios/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _portfolioService.DeleteAsync(id);
        
        if (!result)
            return NotFound();

        return NoContent();
    }
}
