using Remp.Common.Utilities;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken ct = default);
}