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

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize; // protect API

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
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }
}