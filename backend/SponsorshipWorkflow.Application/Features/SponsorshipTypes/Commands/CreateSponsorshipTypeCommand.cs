using MediatR;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

public record CreateSponsorshipTypeCommand(string Name) : IRequest<Result<SponsorshipTypeResponse>>;
