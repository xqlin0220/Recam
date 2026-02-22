using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "photographyCompany")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _userService.GetUsersAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<UserDto>>.Ok(result, "Users fetched successfully."));
    }
}