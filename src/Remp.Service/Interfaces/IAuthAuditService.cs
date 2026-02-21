namespace Remp.Service.Interfaces;

public interface IAuthAuditService
{
    Task LogSuccessAsync(string email, string userId, string role, string? ip, string? userAgent);
    Task LogFailedAsync(string email, string reason, string? ip, string? userAgent);
    Task RegisterSuccessAsync(string email, string userId, string role, string? ip, string? userAgent);
    Task RegisterFailedAsync(string email, string reason, string? ip, string? userAgent);
}