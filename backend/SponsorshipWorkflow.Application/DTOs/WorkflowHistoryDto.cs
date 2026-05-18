using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.DTOs;

public class WorkflowHistoryDto
{
    public Guid Id { get; set; }
    public RequestStatus FromStatus { get; set; }
    public string FromStatusName { get; set; } = string.Empty;
    public RequestStatus ToStatus { get; set; }
    public string ToStatusName { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime ActionedAt { get; set; }
}
