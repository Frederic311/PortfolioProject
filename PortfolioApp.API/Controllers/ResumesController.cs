using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.API.Services;
using PortfolioApp.Shared.DTOs;

namespace PortfolioApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumesController : ControllerBase
{
    private readonly IResumeService _resumeService;

    public ResumesController(IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    // GET: api/resumes/portfolio/1
    [HttpGet("portfolio/{portfolioId}")]
    public async Task<ActionResult<ResumeDto>> GetByPortfolioId(int portfolioId)
    {
    var resume = await _resumeService.GetByPortfolioIdAsync(portfolioId);
        
  if (resume == null)
    return NotFound();
        
        return Ok(resume);
}

    // POST: api/resumes
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ResumeDto>> Upload([FromBody] ResumeDto dto)
    {
      var resume = await _resumeService.UploadAsync(dto);
 return Ok(resume);
    }

    // DELETE: api/resumes/5
[Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
  {
        var result = await _resumeService.DeleteAsync(id);
        
   if (!result)
      return NotFound();
        
        return NoContent();
    }

    // GET: api/resumes/download/1
    [HttpGet("download/{portfolioId}")]
    public async Task<IActionResult> Download(int portfolioId)
    {
        var resume = await _resumeService.GetByPortfolioIdAsync(portfolioId);
        
   if (resume == null)
return NotFound();

    // Convertir Base64 en bytes
        var fileBytes = Convert.FromBase64String(resume.FileContentBase64.Split(',').Last());
        
        return File(fileBytes, resume.ContentType, resume.FileName);
    }
}
