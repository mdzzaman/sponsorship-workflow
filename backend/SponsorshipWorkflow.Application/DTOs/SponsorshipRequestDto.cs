using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.DTOs;

public class SponsorshipRequestDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RequestorId { get; set; } = string.Empty;
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public Guid SponsorshipTypeId { get; set; }
    public string SponsorshipTypeName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Justification { get; set; } = string.Empty;
    public string? ExpectedBenefit { get; set; }
    public string? Remarks { get; set; }
    public RequestStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<WorkflowHistoryDto> WorkflowHistories { get; set; } = new();
}
