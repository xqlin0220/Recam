using MongoDB.Bson;
using MongoDB.Driver;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class AdminAuditLogService : IAdminAuditLogService
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public AdminAuditLogService(IMongoDatabase database)
    {
        _collection = database.GetCollection<BsonDocument>("AdminAuditLogs");
    }

    public async Task LogPasswordChangedAsync(
        string userId,
        string email,
        string role,
        string? ip,
        string? userAgent)
    {
        var log = new BsonDocument
        {
            { "action", "PASSWORD_CHANGED" },
            { "userId", userId },
            { "email", email },
            { "role", role },
            { "ipAddress", ip ?? "" },
            { "userAgent", userAgent ?? "" },
            { "createdAt", DateTime.UtcNow }
        };

        await _collection.InsertOneAsync(log);
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Remp.Service.Services;

public class AdminAuditLogService : Remp.Service.Interfaces.IAdminAuditLogService
{
    private readonly IMongoCollection<dynamic> _collection;

    public AdminAuditLogService(IConfiguration config)
    {
        var mongoConn = config.GetConnectionString("MongoDb");
        var dbName = config["MongoDb:Database"] ?? "RempDb";

        var client = new MongoClient(mongoConn);
        var db = client.GetDatabase(dbName);

        _collection = db.GetCollection<dynamic>("AdminAuditLogs");
    }

    public Task LogAgentCreatedAsync(
        string performedByUserId,
        string performedByEmail,
        string targetAgentUserId,
        string targetAgentEmail,
        string? ip,
        string? userAgent)
    {
        var doc = new
        {
            Action = "AGENT_CREATED",
            UtcTime = DateTime.UtcNow,
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            TargetUserId = targetAgentUserId,
            TargetEmail = targetAgentEmail,
            Ip = ip,
            UserAgent = userAgent
        };

        return _collection.InsertOneAsync(doc);
    }
}