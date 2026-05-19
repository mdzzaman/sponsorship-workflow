namespace SponsorshipWorkflow.API.Contracts.SponsorshipRequests;

public record UpdateSponsorshipRequest(
    string Title,
    string Department,
    Guid SponsorshipTypeId,
    string EventName,
    DateTime EventDate,
    decimal RequestedAmount,
    string Justification,
    string? ExpectedBenefit,
    string? Remarks
);
