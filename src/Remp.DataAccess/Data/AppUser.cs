using Microsoft.AspNetCore.Identity;

namespace Remp.DataAccess.Data;

public class AppUser : IdentityUser
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}