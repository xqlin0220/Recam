namespace Remp.Service.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresUtc { get; set; }

    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!; // "user" or "photographyCompany"
}