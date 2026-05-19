using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Queries;

public record GetSponsorshipTypesQuery(bool ActiveOnly = true) : IRequest<List<SponsorshipTypeResponse>>;
