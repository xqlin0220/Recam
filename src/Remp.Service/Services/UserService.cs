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
    private readonly IAdminAuditLogService _audit;

    public UserService(UserManager<AppUser> userManager, AppDbContext db, IAdminAuditLogService audit)
    {
        _userManager = userManager;
        _db = db;
        _audit = audit;
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

    public async Task ChangePasswordAsync(
        string userId,
        string email,
        string role,
        UpdatePasswordRequest request,
        string? ip,
        string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            throw new ArgumentException("CurrentPassword is required.");

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            throw new ArgumentException("NewPassword is required.");

        if (request.NewPassword.Length < 8)
            throw new ArgumentException("NewPassword must be at least 8 characters.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var msg = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ArgumentException($"Change password failed: {msg}");
        }

        await _audit.LogPasswordChangedAsync(userId, email, role, ip, userAgent);
    }
}