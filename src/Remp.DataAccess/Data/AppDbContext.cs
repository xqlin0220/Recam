using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<Listcase> Listcases => Set<Listcase>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<CaseContact> CaseContacts => Set<CaseContact>();
    public DbSet<PhotographyCompany> PhotographyCompanies => Set<PhotographyCompany>();
    public DbSet<AgentListcase> AgentListcases => Set<AgentListcase>();
    public DbSet<AgentPhotographyCompany> AgentPhotographyCompanies => Set<AgentPhotographyCompany>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}