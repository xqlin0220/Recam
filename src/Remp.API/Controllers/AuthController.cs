using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.DataAccess.Data;
using Remp.Service.DTOs;
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

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        // Basic validation
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            await _audit.RegisterFailedAsync(request.Email ?? "", "EmailOrPasswordEmpty", ip, ua);
            return BadRequest(ApiResponse<RegisterResponse>.Fail("Email and password are required."));
        }

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
        {
            await _audit.RegisterFailedAsync(request.Email, "EmailAlreadyExists", ip, ua);
            return Conflict(ApiResponse<RegisterResponse>.Fail("Email already exists."));
        }

        // Create user (password hashing is done by Identity)
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToList();
            await _audit.RegisterFailedAsync(request.Email, "CreateUserFailed", ip, ua);
            return BadRequest(ApiResponse<RegisterResponse>.Fail("Registration failed.", errors));
        }

        // Distinguish user/admin by Role (agent defaults to "user")
        var role = "user";
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            var errors = roleResult.Errors.Select(e => e.Description).ToList();
            await _audit.RegisterFailedAsync(request.Email, "AddRoleFailed", ip, ua);
            return StatusCode(500, ApiResponse<RegisterResponse>.Fail("Failed to assign role.", errors));
        }

        await _audit.RegisterSuccessAsync(user.Email!, user.Id, role, ip, ua);

        return Ok(ApiResponse<RegisterResponse>.Ok(new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = role
        }, "Registration successful."));
    }
}