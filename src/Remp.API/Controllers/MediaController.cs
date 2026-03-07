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
    private readonly IMediaStorageService _storage;

    public MediaController(
        IMediaService mediaService,
        IMediaStorageService storage)
    {
        _mediaService = mediaService;
        _storage = storage;
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

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string mediaType)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File required");
        }

        await using var stream = file.OpenReadStream();
        var url = await _storage.UploadAsync(stream, file.FileName, file.ContentType, mediaType);

        return Ok(new
        {
            accessUrl = url
        });
    }
}