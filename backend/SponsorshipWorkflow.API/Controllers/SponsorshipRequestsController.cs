using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.API.Authorization;
using SponsorshipWorkflow.API.Contracts.SponsorshipRequests;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SponsorshipRequestsController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("NameIdentifier claim missing on authenticated user.");

    private IReadOnlyList<string> ActorRoles => [.. User.FindAll(ClaimTypes.Role).Select(c => c.Value)];

    private string GetFullName() =>
        User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";

    [HttpGet("my")]
    [Authorize(Policy = Policies.IsRequestor)]
    public async Task<IActionResult> GetMyRequests()
    {
        var result = await mediator.Send(new GetMyRequestsQuery(UserId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetRequestByIdQuery(id, UserId, ActorRoles));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Policies.IsRequestor)]
    public async Task<IActionResult> Create([FromBody] CreateSponsorshipRequest dto)
    {
        var result = await mediator.Send(new CreateRequestCommand(
            dto.Title, UserId, GetFullName(), dto.Department,
            dto.SponsorshipTypeId, dto.EventName, dto.EventDate,
            dto.RequestedAmount, dto.Justification, dto.ExpectedBenefit, dto.Remarks));

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.IsRequestor)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSponsorshipRequest dto)
    {
        var result = await mediator.Send(new UpdateRequestCommand(
            id, UserId, dto.Title, dto.Department, dto.SponsorshipTypeId,
            dto.EventName, dto.EventDate, dto.RequestedAmount, dto.Justification,
            dto.ExpectedBenefit, dto.Remarks));

        return result.IsSuccess ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Policy = Policies.IsRequestor)]
    public async Task<IActionResult> Submit(Guid id)
    {
        var result = await mediator.Send(new SubmitRequestCommand(id, UserId, GetFullName()));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = Policies.IsRequestor)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await mediator.Send(new CancelRequestCommand(id, UserId, GetFullName()));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpGet("pending")]
    [Authorize(Policy = Policies.CanApprove)]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var result = await mediator.Send(new GetPendingApprovalsQuery(ActorRoles));
        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = Policies.CanApprove)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ActionRemarkRequest dto)
    {
        var result = await mediator.Send(new ApproveRequestCommand(id, UserId, GetFullName(), ActorRoles, dto.Remarks));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = Policies.CanApprove)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ActionRemarkRequest dto)
    {
        var result = await mediator.Send(new RejectRequestCommand(id, UserId, GetFullName(), ActorRoles, dto.Remarks));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpGet]
    [Authorize(Policy = Policies.IsSystemAdmin)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllRequestsQuery());
        return Ok(result);
    }
}
