using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data.Configurations;

public class PhotographyCompanyConfiguration : IEntityTypeConfiguration<PhotographyCompany>
{
    public void Configure(EntityTypeBuilder<PhotographyCompany> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(50);

        builder.Property(x => x.PhotographyCompanyName)
            .IsRequired()
            .HasMaxLength(200);
    }
}