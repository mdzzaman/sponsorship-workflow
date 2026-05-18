using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Queries;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SponsorshipTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
    {
        var result = await mediator.Send(new GetSponsorshipTypesQuery(activeOnly));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = UserRole.SystemAdmin)]
    public async Task<IActionResult> Create([FromBody] CreateTypeDto dto)
    {
        var result = await mediator.Send(new CreateSponsorshipTypeCommand(dto.Name));
        return result.IsSuccess ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRole.SystemAdmin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTypeDto dto)
    {
        var result = await mediator.Send(new UpdateSponsorshipTypeCommand(id, dto.Name, dto.IsActive));
        return result.IsSuccess ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

public record CreateTypeDto(string Name);
public record UpdateTypeDto(string Name, bool IsActive);
