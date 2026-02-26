namespace Remp.Service.Interfaces;

public interface IPhotographyCompanyService
{
    Task AddAgentAsync(string photographyCompanyUserId, string agentId);
}