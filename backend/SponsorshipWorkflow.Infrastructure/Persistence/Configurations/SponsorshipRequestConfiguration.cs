using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Persistence.Configurations;

public class SponsorshipRequestConfiguration : IEntityTypeConfiguration<SponsorshipRequest>
{
    public void Configure(EntityTypeBuilder<SponsorshipRequest> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.RequestorId).HasMaxLength(450).IsRequired();
        builder.Property(r => r.RequestorName).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Department).HasMaxLength(100).IsRequired();
        builder.Property(r => r.EventName).HasMaxLength(200).IsRequired();
        builder.Property(r => r.RequestedAmount).HasPrecision(18, 2);
        builder.Property(r => r.Justification).HasMaxLength(2000).IsRequired();
        builder.Property(r => r.ExpectedBenefit).HasMaxLength(2000);
        builder.Property(r => r.Remarks).HasMaxLength(1000);

        builder.HasOne(r => r.SponsorshipType)
            .WithMany(t => t.SponsorshipRequests)
            .HasForeignKey(r => r.SponsorshipTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.WorkflowHistories)
            .WithOne(h => h.Request)
            .HasForeignKey(h => h.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // xmin is a PostgreSQL system column that auto-increments on every row write.
        // EF appends WHERE xmin = <read_value> on every UPDATE — no migration column needed.
        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Ignore(r => r.DomainEvents);
    }
}
