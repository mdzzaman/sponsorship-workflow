using MediatR;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

public record CreateSponsorshipTypeCommand(string Name) : IRequest<Result<SponsorshipTypeDto>>;
