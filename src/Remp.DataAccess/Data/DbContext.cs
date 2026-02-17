using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options) { }

    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<ListingCase> ListingCases => Set<ListingCase>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<CaseContact> CaseContacts => Set<CaseContact>();
    public DbSet<PhotographyCompany> PhotographyCompanies => Set<PhotographyCompany>();
    public DbSet<AgentListingCase> AgentListingCases => Set<AgentListingCase>();
    public DbSet<AgentPhotographyCompany> AgentPhotographyCompanies => Set<AgentPhotographyCompany>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);
    }
}