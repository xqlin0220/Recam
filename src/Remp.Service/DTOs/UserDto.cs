namespace Remp.Service.DTOs;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}