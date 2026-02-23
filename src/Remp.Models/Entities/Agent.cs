namespace Remp.Models.Entities;

public class Agent
{
    public string Id { get; set; } = default!;   // PK and FK

    public string AgentFirstName { get; set; } = default!;

    public string AgentLastName { get; set; } = default!;

    public string? AvatarUrl { get; set; }

    public string CompanyName { get; set; } = default!;

    public ICollection<AgentListcase> AgentListcases { get; set; } = new List<AgentListcase>();
    public ICollection<AgentPhotographyCompany> AgentPhotographyCompanies { get; set; } = new List<AgentPhotographyCompany>();
}