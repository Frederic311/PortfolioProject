using Microsoft.EntityFrameworkCore;
using PortfolioApp.Shared.Models;

namespace PortfolioApp.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Tool> Tools { get; set; }
    public DbSet<ProjectImage> ProjectImages { get; set; }
    public DbSet<ProjectTool> ProjectTools { get; set; }
    public DbSet<Resume> Resumes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Portfolio -> Projects (One-to-Many)
        modelBuilder.Entity<Portfolio>()
            .HasMany(p => p.Projects)
            .WithOne()
            .HasForeignKey(p => p.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Portfolio -> Skills (One-to-Many)
        modelBuilder.Entity<Portfolio>()
            .HasMany(p => p.Skills)
            .WithOne()
            .HasForeignKey(s => s.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Skill -> Tools (One-to-Many)
        modelBuilder.Entity<Skill>()
            .HasMany(s => s.Tools)
            .WithOne()
            .HasForeignKey(t => t.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project -> ProjectImages (One-to-Many)
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project <-> Tool (Many-to-Many via ProjectTool)
        modelBuilder.Entity<ProjectTool>()
            .HasKey(pt => pt.Id);

        modelBuilder.Entity<ProjectTool>()
            .HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTools)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectTool>()
            .HasOne(pt => pt.Tool)
            .WithMany()
            .HasForeignKey(pt => pt.ToolId)
            .OnDelete(DeleteBehavior.Restrict); // Évite les cycles de cascade
            
        // Configuration de l'enum pour être stocké comme string
        modelBuilder.Entity<Project>()
            .Property(p => p.Status)
            .HasConversion<string>();
    }
}
