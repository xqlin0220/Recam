using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Recam.Domain.Entities;
using Recam.Infrastructure.Identity;

namespace Recam.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Core tables
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<ListingCase> ListingCases => Set<ListingCase>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<CaseContact> CaseContacts => Set<CaseContact>();
    public DbSet<PhotographyCompany> PhotographyCompanies => Set<PhotographyCompany>();

    // Join tables (many-to-many)
    public DbSet<AgentListingCase> AgentListingCases => Set<AgentListingCase>();
    public DbSet<AgentPhotographyCompany> AgentPhotographyCompanies => Set<AgentPhotographyCompany>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 先留空：下一步在这里挂 Fluent API 配置
    }
}