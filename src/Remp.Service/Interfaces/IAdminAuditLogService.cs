

namespace Remp.Service.Interfaces;

public interface IAdminAuditLogService
{
    Task LogPasswordChangedAsync(
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent);
}