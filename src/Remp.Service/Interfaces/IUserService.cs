
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken ct = default);
    Task<CurrentUserDto> GetMeAsync(string userId, string email, string role);

    Task ChangePasswordAsync(
        string userId,
        string email,
        string role,
        UpdatePasswordRequest request,
        string? ip,
        string? userAgent);
}