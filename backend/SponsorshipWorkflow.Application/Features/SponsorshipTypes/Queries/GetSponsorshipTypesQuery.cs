using MediatR;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Queries;

public record GetSponsorshipTypesQuery(bool ActiveOnly = true) : IRequest<List<SponsorshipTypeDto>>;
