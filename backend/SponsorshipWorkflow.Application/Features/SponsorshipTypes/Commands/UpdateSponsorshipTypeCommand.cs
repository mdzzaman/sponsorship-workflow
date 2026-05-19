using MediatR;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

public record UpdateSponsorshipTypeCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<SponsorshipTypeResponse>>;
