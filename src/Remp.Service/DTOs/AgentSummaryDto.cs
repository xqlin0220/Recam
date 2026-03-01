namespace Remp.Service.DTOs;

public class AgentSummaryDto
{
    public string AgentUserId { get; set; } = default!;
    public string Email { get; set; } = default!;

    public string? AgentFirstName { get; set; }
    public string? AgentLastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CompanyName { get; set; }
}