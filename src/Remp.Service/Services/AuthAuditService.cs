using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Remp.DataAccess.Collections;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class AuthAuditService : IAuthAuditService
{
    private readonly IMongoCollection<AuthEvent> _collection;

    public AuthAuditService(IConfiguration config)
    {
        var mongo = config.GetSection("MongoAudit");
        var client = new MongoClient(mongo["ConnectionString"]);
        var db = client.GetDatabase(mongo["Database"]);
        _collection = db.GetCollection<AuthEvent>(mongo["Collection"]);
    }

    public Task LogSuccessAsync(string email, string userId, string role, string? ip, string? userAgent)
        => _collection.InsertOneAsync(new AuthEvent
        {
            EventType = "LOGIN_SUCCESS",
            Email = email,
            UserId = userId,
            Role = role,
            Ip = ip,
            UserAgent = userAgent
        });

    public Task LogFailedAsync(string email, string reason, string? ip, string? userAgent)
        => _collection.InsertOneAsync(new AuthEvent
        {
            EventType = "LOGIN_FAILED",
            Email = email,
            Reason = reason,
            Ip = ip,
            UserAgent = userAgent
        });

    public Task RegisterSuccessAsync(string email, string userId, string role, string? ip, string? userAgent)
        => _collection.InsertOneAsync(new AuthEvent
        {
            EventType = "REGISTER_SUCCESS",
            Email = email,
            UserId = userId,
            Role = role,
            Ip = ip,
            UserAgent = userAgent
        });

    public Task RegisterFailedAsync(string email, string reason, string? ip, string? userAgent)
        => _collection.InsertOneAsync(new AuthEvent
        {
            EventType = "REGISTER_FAILED",
            Email = email,
            Reason = reason,
            Ip = ip,
            UserAgent = userAgent
        });
}