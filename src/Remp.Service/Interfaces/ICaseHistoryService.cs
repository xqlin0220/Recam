namespace Remp.Service.Interfaces;

public interface ICaseHistoryService
{
    Task LogCaseCreatedAsync(
        int listcaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? snapshot = null);

    Task LogCaseUpdatedAsync(
        int listcaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? changes = null);

    Task LogCaseDeletedAsync(
        int listcaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? snapshot = null);
}