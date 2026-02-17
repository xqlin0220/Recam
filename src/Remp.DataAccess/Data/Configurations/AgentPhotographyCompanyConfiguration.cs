using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data.Configurations;

public class AgentPhotographyCompanyConfiguration
    : IEntityTypeConfiguration<AgentPhotographyCompany>
{
    public void Configure(EntityTypeBuilder<AgentPhotographyCompany> builder)
    {
        // Composite Primary Key
        builder.HasKey(x => new { x.AgentId, x.PhotographyCompanyId });

        builder.Property(x => x.AgentId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PhotographyCompanyId)
            .HasMaxLength(50)
            .IsRequired();

        // FK -> Agent
        builder.HasOne(x => x.Agent)
            .WithMany(x => x.AgentPhotographyCompanies)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK -> PhotographyCompany
        builder.HasOne(x => x.PhotographyCompany)
            .WithMany(x => x.AgentPhotographyCompanies)
            .HasForeignKey(x => x.PhotographyCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}