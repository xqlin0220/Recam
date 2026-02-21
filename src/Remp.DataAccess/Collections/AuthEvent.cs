namespace Remp.DataAccess.Collections;

public class AuthEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime UtcTime { get; set; } = DateTime.UtcNow;

    public string EventType { get; set; } = default!; // LOGIN_SUCCESS / LOGIN_FAILED
    public string Email { get; set; } = default!;
    public string? UserId { get; set; }
    public string? Role { get; set; }

    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public string? Reason { get; set; }
}