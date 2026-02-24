using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Remp.DataAccess.Collections;
using Remp.Service.Interfaces;
using Remp.Models.Enums;

namespace Remp.Service.Services;

public class CaseHistoryService : ICaseHistoryService
{
    private readonly IMongoCollection<CaseHistory> _collection;

    public CaseHistoryService(IConfiguration config)
    {
        var section = config.GetSection("MongoAudit");
        var client = new MongoClient(section["ConnectionString"]);
        var db = client.GetDatabase(section["Database"]);

        _collection = db.GetCollection<CaseHistory>("CaseHistory");
    }

    public Task LogCaseCreatedAsync(
        int ListcaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? snapshot = null)
    {
        return _collection.InsertOneAsync(new CaseHistory
        {
            ListcaseId = ListcaseId,
            Action = "CASE_CREATED",
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Role = role,
            Ip = ip,
            UserAgent = userAgent,
            Snapshot = snapshot
        });
    }

    public Task LogCaseUpdatedAsync(
        int ListcaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? changes = null)
    {
        return _collection.InsertOneAsync(new CaseHistory
        {
            ListcaseId = ListcaseId,
            Action = "CASE_UPDATED",
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Role = role,
            Ip = ip,
            UserAgent = userAgent,
            Snapshot = changes
        });
    }

    public Task LogCaseDeletedAsync(
        int listcaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? snapshot = null)
    {
        return _collection.InsertOneAsync(new CaseHistory
        {
            ListcaseId = listcaseId,   
            Action = "CASE_DELETED",
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Role = role,
            Ip = ip,
            UserAgent = userAgent,
            Snapshot = snapshot
        });
    }

    public Task LogStatusChangedAsync(
        int listcaseId,
        ListcaseStatus from,
        ListcaseStatus to,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent)
    {
        return _collection.InsertOneAsync(new CaseHistory
        {
            ListcaseId = listcaseId,
            Action = "STATUS_CHANGED",
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Role = role,
            Ip = ip,
            UserAgent = userAgent,
            Snapshot = new
            {
                From = from.ToString(),
                To = to.ToString()
            }
        });
    }
}