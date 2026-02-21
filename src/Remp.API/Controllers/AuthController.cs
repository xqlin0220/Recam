using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.DataAccess.Data;
using Remp.Service.DTOs.Auth;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthAuditService _audit;

    public AuthController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService,
        IAuthAuditService audit)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _audit = audit;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.IsDeleted)
        {
            await _audit.LogFailedAsync(request.Email, "UserNotFoundOrDeleted", ip, ua);
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid credentials."));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            var reason = result.IsLockedOut ? "LockedOut" : "BadPassword";
            await _audit.LogFailedAsync(request.Email, reason, ip, ua);
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid credentials."));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        // Must distinguish User and Admin after login
        if (role != "user" && role != "photographyCompany")
        {
            await _audit.LogFailedAsync(request.Email, "InvalidRole", ip, ua);
            return StatusCode(403, ApiResponse<LoginResponse>.Fail("Access denied."));
        }

        var (token, expiresUtc) = _jwtTokenService.CreateToken(user.Id, user.Email!, role);

        await _audit.LogSuccessAsync(user.Email!, user.Id, role, ip, ua);

        var response = new LoginResponse
        {
            Token = token,
            ExpiresUtc = expiresUtc,
            UserId = user.Id,
            Email = user.Email!,
            Role = role
        };

        return Ok(ApiResponse<LoginResponse>.Ok(response, "Login successful."));
    }
}