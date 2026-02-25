namespace Remp.Service.DTOs;

public class CurrentUserDto
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;

    public List<int> ListingIds { get; set; } = new();
}