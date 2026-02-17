using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MediaUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(50);

        // ListingCase 1 - many MediaAsset
        builder.HasOne(x => x.ListingCase)
            .WithMany(x => x.MediaAssets)
            .HasForeignKey(x => x.ListingCaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}