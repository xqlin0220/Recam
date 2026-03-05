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
    }
}