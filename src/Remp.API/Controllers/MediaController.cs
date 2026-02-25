using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "";
        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "";
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        await _mediaService.DeleteAsync(id, userId, email, role, ip, ua);

        return Ok(ApiResponse<object>.Ok(new { id }, $"Media {id} deleted successfully."));
    }
}