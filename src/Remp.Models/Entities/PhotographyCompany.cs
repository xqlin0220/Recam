namespace Remp.Models.Entities;

public class PhotographyCompany
{
    public string Id { get; set; } = default!;   // PK

    public string PhotographyCompanyName { get; set; } = default!;

    // Navigation - many Agents
    public ICollection<AgentPhotographyCompany> AgentPhotographyCompanies { get; set; }
        = new List<AgentPhotographyCompany>();
}