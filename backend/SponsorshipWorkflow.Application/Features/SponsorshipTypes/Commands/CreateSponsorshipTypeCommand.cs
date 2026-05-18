using MediatR;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

public record CreateSponsorshipTypeCommand(string Name) : IRequest<Result<SponsorshipTypeDto>>;
