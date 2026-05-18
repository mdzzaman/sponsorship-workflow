using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SponsorshipRequest> SponsorshipRequests { get; }
    DbSet<WorkflowHistory> WorkflowHistories { get; }
    DbSet<SponsorshipType> SponsorshipTypes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
