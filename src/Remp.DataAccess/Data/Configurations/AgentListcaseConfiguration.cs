using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data.Configurations;

public class AgentListcaseConfiguration : IEntityTypeConfiguration<AgentListcase>
{
    public void Configure(EntityTypeBuilder<AgentListcase> builder)
    {
        builder.HasKey(x => new { x.AgentId, x.ListcaseId });

        builder.Property(x => x.AgentId)
            .HasMaxLength(50);

        builder.HasOne(x => x.Agent)
            .WithMany(x => x.AgentListcases)
            .HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Listcase)
            .WithMany(x => x.AgentListcases)
            .HasForeignKey(x => x.ListcaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}