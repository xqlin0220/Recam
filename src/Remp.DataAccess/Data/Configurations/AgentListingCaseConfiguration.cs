using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data.Configurations;

public class AgentListingCaseConfiguration : IEntityTypeConfiguration<AgentListingCase>
{
    public void Configure(EntityTypeBuilder<AgentListingCase> builder)
    {
        builder.HasKey(x => new { x.AgentId, x.ListingCaseId });

        builder.Property(x => x.AgentId)
            .HasMaxLength(50);

        builder.HasOne(x => x.Agent)
            .WithMany(x => x.AgentListingCases)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ListingCase)
            .WithMany(x => x.AgentListingCases)
            .HasForeignKey(x => x.ListingCaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}