using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.DataAccess.Data;

namespace Remp.Service.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;

    public UserService(UserManager<AppUser> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _userManager.Users.AsNoTracking();

        // Optional: exclude soft deleted users
        query = query.Where(u => !u.IsDeleted);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email!,
                CreatedAt = u.CreatedAt,
                IsDeleted = u.IsDeleted
            })
            .ToListAsync(ct);

        return new PagedResult<UserDto>
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<CurrentUserDto> GetMeAsync(string userId, string email, string role)
    {
        List<int> listingIds;

        if (role == "photographyCompany")
        {
            // Listcases created by this user
            listingIds = await _db.Listcases
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(x => x.Id)
                .ToListAsync();
        }
        else if (role == "user")
        {
            // Listcases assigned to this agent
            listingIds = await _db.AgentListcases
                .AsNoTracking()
                .Where(x => x.AgentId == userId)
                .Select(x => x.ListcaseId)
                .Distinct()
                .ToListAsync();
        }
        else
        {
            listingIds = new List<int>();
        }

        return new CurrentUserDto
        {
            UserId = userId,
            Email = email,
            Role = role,
            ListingIds = listingIds
        };
    }
}