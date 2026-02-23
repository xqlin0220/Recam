using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Remp.DataAccess.Collections;
using Remp.Service.Interfaces;

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
        int listingCaseId,
        string performedByUserId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent,
        object? snapshot = null)
    {
        return _collection.InsertOneAsync(new CaseHistory
        {
            ListingCaseId = listingCaseId,
            Action = "CASE_CREATED",
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Role = role,
            Ip = ip,
            UserAgent = userAgent,
            Snapshot = snapshot
        });
    }
}