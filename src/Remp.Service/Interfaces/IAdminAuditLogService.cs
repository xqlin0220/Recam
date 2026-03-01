namespace Remp.Service.Interfaces;

public interface IAdminAuditLogService
{
    Task LogAgentCreatedAsync(
        string performedByUserId,
        string performedByEmail,
        string targetAgentUserId,
        string targetAgentEmail,
        string? ip,
        string? userAgent);
}