namespace Remp.Service.DTOs;

public class CreateAgentRequest
{
    public string Email { get; set; } = default!;
    public string AgentFirstName { get; set; } = default!;
    public string AgentLastName { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
}