using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Remp.Service.DTOs.Audit;

public class AdminAuditLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Action { get; set; } = default!; // e.g. "AGENT_CREATED"
    public DateTime UtcTime { get; set; } = DateTime.UtcNow;

    public string PerformedByUserId { get; set; } = default!;
    public string PerformedByEmail { get; set; } = default!;
    public string Role { get; set; } = default!;

    public string TargetUserId { get; set; } = default!;
    public string TargetEmail { get; set; } = default!;

    public string? Ip { get; set; }
    public string? UserAgent { get; set; }

    public object? Snapshot { get; set; }
}