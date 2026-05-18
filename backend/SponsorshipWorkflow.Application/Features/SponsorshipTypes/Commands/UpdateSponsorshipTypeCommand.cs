using MediatR;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

public record UpdateSponsorshipTypeCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<SponsorshipTypeDto>>;
