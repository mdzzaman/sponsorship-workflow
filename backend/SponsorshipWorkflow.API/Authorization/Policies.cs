namespace SponsorshipWorkflow.API.Authorization;

public static class Policies
{
    public const string IsRequestor = nameof(IsRequestor);
    public const string CanApprove = nameof(CanApprove);
    public const string IsSystemAdmin = nameof(IsSystemAdmin);
}
