using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

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

        // Composite PK: AgentListingCase (AgentId + ListingCaseId)
        modelBuilder.Entity<AgentListingCase>()
            .HasKey(x => new { x.AgentId, x.ListingCaseId });

        // Composite PK: AgentPhotographyCompany (AgentId + PhotographyCompanyId)
        modelBuilder.Entity<AgentPhotographyCompany>()
            .HasKey(x => new { x.AgentId, x.PhotographyCompanyId });

        // ListingCase 1 - many MediaAsset
        modelBuilder.Entity<MediaAsset>()
            .HasOne(x => x.ListingCase)
            .WithMany(x => x.MediaAssets)
            .HasForeignKey(x => x.ListingCaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // ListingCase 1 - many CaseContact
        modelBuilder.Entity<CaseContact>()
            .HasOne(x => x.ListingCase)
            .WithMany(x => x.CaseContacts)
            .HasForeignKey(x => x.ListingCaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // AgentListingCase -> Agent
        modelBuilder.Entity<AgentListingCase>()
            .HasOne(x => x.Agent)
            .WithMany(x => x.AgentListingCases)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // AgentListingCase -> ListingCase
        modelBuilder.Entity<AgentListingCase>()
            .HasOne(x => x.ListingCase)
            .WithMany(x => x.AgentListingCases)
            .HasForeignKey(x => x.ListingCaseId)
            .OnDelete(DeleteBehavior.Restrict);

        // AgentPhotographyCompany -> Agent
        modelBuilder.Entity<AgentPhotographyCompany>()
            .HasOne(x => x.Agent)
            .WithMany(x => x.AgentPhotographyCompanies)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // AgentPhotographyCompany -> PhotographyCompany
        modelBuilder.Entity<AgentPhotographyCompany>()
            .HasOne(x => x.PhotographyCompany)
            .WithMany(x => x.AgentPhotographyCompanies)
            .HasForeignKey(x => x.PhotographyCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}