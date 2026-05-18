namespace SponsorshipWorkflow.Application.DTOs;

public class SponsorshipTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
