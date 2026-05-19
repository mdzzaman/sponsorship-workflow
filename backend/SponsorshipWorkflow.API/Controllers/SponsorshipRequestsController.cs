using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.API.Contracts.SponsorshipRequests;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SponsorshipRequestsController(IMediator mediator, UserManager<IdentityUser> userManager) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("NameIdentifier claim missing on authenticated user.");

    private async Task<string> GetFullNameAsync()
    {
        var user = await userManager.FindByIdAsync(UserId);
        var claims = user != null ? await userManager.GetClaimsAsync(user) : [];
        return claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
    }

    [HttpGet("my")]
    [Authorize(Roles = UserRole.Requestor)]
    public async Task<IActionResult> GetMyRequests()
    {
        var result = await mediator.Send(new GetMyRequestsQuery(UserId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetRequestByIdQuery(id));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Requestor)]
    public async Task<IActionResult> Create([FromBody] CreateSponsorshipRequest dto)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new CreateRequestCommand(
            dto.Title, UserId, fullName, dto.Department,
            dto.SponsorshipTypeId, dto.EventName, dto.EventDate,
            dto.RequestedAmount, dto.Justification, dto.ExpectedBenefit, dto.Remarks));

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRole.Requestor)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSponsorshipRequest dto)
    {
        var result = await mediator.Send(new UpdateRequestCommand(
            id, UserId, dto.Title, dto.Department, dto.SponsorshipTypeId,
            dto.EventName, dto.EventDate, dto.RequestedAmount, dto.Justification,
            dto.ExpectedBenefit, dto.Remarks));

        return result.IsSuccess ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = UserRole.Requestor)]
    public async Task<IActionResult> Submit(Guid id)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new SubmitRequestCommand(id, UserId, fullName));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = UserRole.Requestor)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new CancelRequestCommand(id, UserId, fullName));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpGet("pending-manager")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> GetPendingManagerApprovals()
    {
        var result = await mediator.Send(new GetPendingManagerApprovalsQuery());
        return Ok(result);
    }

    [HttpPost("{id:guid}/manager-approve")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> ManagerApprove(Guid id, [FromBody] ActionRemarkRequest dto)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new ApproveByManagerCommand(id, UserId, fullName, dto.Remarks));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/manager-reject")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> ManagerReject(Guid id, [FromBody] ActionRemarkRequest dto)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new RejectByManagerCommand(id, UserId, fullName, dto.Remarks!));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpGet("pending-finance")]
    [Authorize(Roles = UserRole.FinanceAdmin)]
    public async Task<IActionResult> GetPendingFinanceReview()
    {
        var result = await mediator.Send(new GetPendingFinanceReviewQuery());
        return Ok(result);
    }

    [HttpPost("{id:guid}/finance-approve")]
    [Authorize(Roles = UserRole.FinanceAdmin)]
    public async Task<IActionResult> FinanceApprove(Guid id, [FromBody] ActionRemarkRequest dto)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new ApproveByFinanceCommand(id, UserId, fullName, dto.Remarks));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/finance-reject")]
    [Authorize(Roles = UserRole.FinanceAdmin)]
    public async Task<IActionResult> FinanceReject(Guid id, [FromBody] ActionRemarkRequest dto)
    {
        var fullName = await GetFullNameAsync();
        var result = await mediator.Send(new RejectByFinanceCommand(id, UserId, fullName, dto.Remarks!));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpGet]
    [Authorize(Roles = UserRole.SystemAdmin)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllRequestsQuery());
        return Ok(result);
    }
}
