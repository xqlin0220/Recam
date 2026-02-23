namespace Remp.Service.Interfaces;

public interface ICaseHistoryService
{
    Task LogCaseCreatedAsync(
        int listingCaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? snapshot = null);
}