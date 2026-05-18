using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Infrastructure.Persistence.Configurations;

public class WorkflowHistoryConfiguration : IEntityTypeConfiguration<WorkflowHistory>
{
    public void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.ActorId).HasMaxLength(450).IsRequired();
        builder.Property(h => h.ActorName).HasMaxLength(200).IsRequired();
        builder.Property(h => h.Remarks).HasMaxLength(1000);
    }
}
