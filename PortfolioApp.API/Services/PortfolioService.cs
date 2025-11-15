using Microsoft.EntityFrameworkCore;
using PortfolioApp.API.Data;
using PortfolioApp.Shared.DTOs;
using PortfolioApp.Shared.Models;

namespace PortfolioApp.API.Services;

public class PortfolioService : IPortfolioService
{
    private readonly AppDbContext _context;

    public PortfolioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PortfolioDto>> GetAllAsync()
    {
        return await _context.Portfolios
            .Select(p => new PortfolioDto
            {
   Id = p.Id,
   Name = p.Name,
    Description = p.Description,
   CreatedDate = p.CreatedDate,
        Title = p.Title,
          Role = p.Role,
      Bio = p.Bio,
            ProfileImage = p.ProfileImage,
       ContactEmail = p.ContactEmail,
         LinkedIn = p.LinkedIn,
     Github = p.Github
            })
            .ToListAsync();
    }

    public async Task<PortfolioDto?> GetByIdAsync(int id)
    {
   var portfolio = await _context.Portfolios
        .FirstOrDefaultAsync(p => p.Id == id);

        if (portfolio == null)
return null;

      return new PortfolioDto
        {
        Id = portfolio.Id,
     Name = portfolio.Name,
 Description = portfolio.Description,
  CreatedDate = portfolio.CreatedDate,
    Title = portfolio.Title,
            Role = portfolio.Role,  // Nouveau champ
            Bio = portfolio.Bio,
            ProfileImage = portfolio.ProfileImage,
  ContactEmail = portfolio.ContactEmail,
            LinkedIn = portfolio.LinkedIn,
            Github = portfolio.Github
        };
    }

    public async Task<PortfolioDto> CreateAsync(CreatePortfolioDto dto)
    {
        var portfolio = new Portfolio
        {
     Name = dto.Name,
            Description = dto.Description,
            Title = dto.Title,
            Role = dto.Role,
       Bio = dto.Bio,
    ProfileImage = dto.ProfileImage,
  ContactEmail = dto.ContactEmail,
      LinkedIn = dto.LinkedIn,
         Github = dto.Github,
   CreatedDate = DateTime.Now
    };

    _context.Portfolios.Add(portfolio);
     await _context.SaveChangesAsync();

        return new PortfolioDto
  {
         Id = portfolio.Id,
     Name = portfolio.Name,
      Description = portfolio.Description,
       CreatedDate = portfolio.CreatedDate,
          Title = portfolio.Title,
      Role = portfolio.Role,
 Bio = portfolio.Bio,
    ProfileImage = portfolio.ProfileImage,
         ContactEmail = portfolio.ContactEmail,
      LinkedIn = portfolio.LinkedIn,
       Github = portfolio.Github
        };
    }

    public async Task<PortfolioDto?> UpdateAsync(int id, UpdatePortfolioDto dto)
    {
 var portfolio = await _context.Portfolios.FindAsync(id);
  
 if (portfolio == null)
  return null;

     portfolio.Name = dto.Name;
      portfolio.Description = dto.Description;
        portfolio.Title = dto.Title;
 portfolio.Role = dto.Role;  // Nouveau champ
        portfolio.Bio = dto.Bio;
   portfolio.ProfileImage = dto.ProfileImage;
        portfolio.ContactEmail = dto.ContactEmail;
        portfolio.LinkedIn = dto.LinkedIn;
    portfolio.Github = dto.Github;

        await _context.SaveChangesAsync();

        return new PortfolioDto
        {
   Id = portfolio.Id,
  Name = portfolio.Name,
Description = portfolio.Description,
     CreatedDate = portfolio.CreatedDate,
 Title = portfolio.Title,
  Role = portfolio.Role,
 Bio = portfolio.Bio,
  ProfileImage = portfolio.ProfileImage,
 ContactEmail = portfolio.ContactEmail,
     LinkedIn = portfolio.LinkedIn,
      Github = portfolio.Github
        };
    }

    public async Task<bool> DeleteAsync(int id)
  {
   var portfolio = await _context.Portfolios.FindAsync(id);
        
        if (portfolio == null)
  return false;

      _context.Portfolios.Remove(portfolio);
     await _context.SaveChangesAsync();
        
        return true;
    }
}
