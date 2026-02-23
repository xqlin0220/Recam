namespace Remp.DataAccess.Collections;

public class CaseHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime UtcTime { get; set; } = DateTime.UtcNow;

    public int ListcaseId { get; set; }                 // SQL Listing ID
    public string Action { get; set; } = default!;         // "CASE_CREATED"
    public string PerformedByUserId { get; set; } = default!;
    public string PerformedByEmail { get; set; } = default!;
    public string Role { get; set; } = default!;           // photographyCompany

    public string? Ip { get; set; }
    public string? UserAgent { get; set; }

    public object? Snapshot { get; set; }                  // optional: store request fields
}