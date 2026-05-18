using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public static class MappingExtensions
{
    public static SponsorshipRequestDto ToDto(this SponsorshipRequest r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        RequestorId = r.RequestorId,
        RequestorName = r.RequestorName,
        Department = r.Department,
        SponsorshipTypeId = r.SponsorshipTypeId,
        SponsorshipTypeName = r.SponsorshipType?.Name ?? string.Empty,
        EventName = r.EventName,
        EventDate = r.EventDate,
        RequestedAmount = r.RequestedAmount,
        Justification = r.Justification,
        ExpectedBenefit = r.ExpectedBenefit,
        Remarks = r.Remarks,
        Status = r.Status,
        StatusName = r.Status.ToString(),
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        WorkflowHistories = r.WorkflowHistories
            .OrderBy(h => h.ActionedAt)
            .Select(h => h.ToDto())
            .ToList()
    };

    public static WorkflowHistoryDto ToDto(this WorkflowHistory h) => new()
    {
        Id = h.Id,
        FromStatus = h.FromStatus,
        FromStatusName = h.FromStatus.ToString(),
        ToStatus = h.ToStatus,
        ToStatusName = h.ToStatus.ToString(),
        ActorName = h.ActorName,
        Remarks = h.Remarks,
        ActionedAt = h.ActionedAt
    };
}
