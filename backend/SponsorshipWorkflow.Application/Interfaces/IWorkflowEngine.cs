using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Interfaces;

public interface IWorkflowEngine
{
    bool CanTransition(RequestStatus current, RequestStatus next, string role);
    RequestStatus GetNextStatus(RequestStatus current, string action);
}
