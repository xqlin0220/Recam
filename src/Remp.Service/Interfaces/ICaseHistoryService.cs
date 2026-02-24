namespace Remp.Service.Interfaces;
using Remp.Models.Enums;

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

    Task LogStatusChangedAsync(
        int listcaseId,
        ListcaseStatus from,
        ListcaseStatus to,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent);
}