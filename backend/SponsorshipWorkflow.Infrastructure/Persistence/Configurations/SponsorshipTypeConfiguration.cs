using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Persistence.Configurations;

public class SponsorshipTypeConfiguration : IEntityTypeConfiguration<SponsorshipType>
{
    public void Configure(EntityTypeBuilder<SponsorshipType> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);

        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
