

namespace Remp.Service.Interfaces;

public interface IAdminAuditLogService
{
    Task LogPasswordChangedAsync(
        string performedByUserId,
        string performedByEmail,
        string role,
    Task LogAgentCreatedAsync(
        string performedByUserId,
        string performedByEmail,
        string targetAgentUserId,
        string targetAgentEmail,
        string? ip,
        string? userAgent);
}