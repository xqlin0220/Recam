namespace Recam.Domain.Entities;

public class Agent
{
    public string Id { get; set; } = default!;   // PK and FK

    public string AgentFirstName { get; set; } = default!;

    public string AgentLastName { get; set; } = default!;

    public string? AvatarUrl { get; set; }

    public string CompanyName { get; set; } = default!;
}