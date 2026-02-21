namespace Remp.Service.DTOs;

public class RegisterResponse
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}