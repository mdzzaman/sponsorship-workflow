namespace SponsorshipWorkflow.Application.Responses;

public class SponsorshipTypeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
