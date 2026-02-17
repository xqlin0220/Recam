using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data.Configurations;

public class CaseContactConfiguration : IEntityTypeConfiguration<CaseContact>
{
    public void Configure(EntityTypeBuilder<CaseContact> builder)
    {
        builder.HasKey(x => x.ContactId);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200);

        builder.Property(x => x.ProfileUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        // ListingCase 1 - many CaseContact
        builder.HasOne(x => x.ListingCase)
            .WithMany(x => x.CaseContacts)
            .HasForeignKey(x => x.ListingCaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}