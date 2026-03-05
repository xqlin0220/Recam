using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IAgentService
{
    Task<CreateAgentResponse> CreateAgentAsync(
        CreateAgentRequest request,
        string photographyCompanyId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent);
}