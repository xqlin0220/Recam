using Remp.Service.DTOs;
namespace Remp.Service.Interfaces;

public interface IPhotographyCompanyService
{
    Task AddAgentAsync(string photographyCompanyUserId, string agentId);
    Task<List<AgentSummaryDto>> GetAgentsAsync(string photographyCompanyUserId);
}