using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    [HttpGet("me")]
    [Authorize(Roles = "photographyCompany,user")]
    public async Task<ActionResult<ApiResponse<CurrentUserDto>>> Me()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
            return Unauthorized(ApiResponse<CurrentUserDto>.Fail("Invalid token."));

        var result = await _userService.GetMeAsync(userId, email ?? "", role);
        return Ok(ApiResponse<CurrentUserDto>.Ok(result));
    }
}