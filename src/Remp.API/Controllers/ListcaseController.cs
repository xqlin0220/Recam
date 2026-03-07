using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListcaseController : ControllerBase
{
    private readonly IMediaAssetService _mediaAssetService;
    private readonly ISelectedMediaService _selectedMediaService;

    public ListcaseController(IMediaAssetService mediaAssetService, ISelectedMediaService selectedMediaService)
    {
        _mediaAssetService = mediaAssetService;
        _selectedMediaService = selectedMediaService;
    }

    [HttpPut("{id:int}/cover-image")]
    [Authorize(Roles = "agent")]
    public async Task<ActionResult<ApiResponse<object>>> SetCoverImage(
        int id,
        [FromBody] SetCoverImageRequestDto request)
    {
        await _mediaAssetService.SetCoverImageAsync(id, request.MediaId);

        return Ok(ApiResponse<object>.Ok(
            new { ListingId = id, request.MediaId },
            "Cover image updated successfully."));
    }

    [HttpGet("{id:int}/final-selection")]
    [Authorize(Roles = "photographyCompany,agent")]
    public async Task<ActionResult<ApiResponse<object>>> GetFinalSelection(int id)
    {
        var result = await _selectedMediaService.GetFinalSelectedMediaAsync(id);

        return Ok(ApiResponse<object>.Ok(result, "Final selected media retrieved successfully."));
    }

    [HttpPut("{id:int}/selected-media")]
    [Authorize(Roles = "agent")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSelectedMedia(
        int id,
        [FromBody] UpdateSelectedMediaRequestDto request)
    {
        var agentId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;

        await _mediaAssetService.UpdateSelectedMediaAsync(id, request.MediaIds, agentId);

        return Ok(ApiResponse<object>.Ok(
            new
            {
                ListingId = id,
                SelectedCount = request.MediaIds.Distinct().Count(),
                MediaIds = request.MediaIds.Distinct().ToList()
            },
            "Selected media updated successfully."));
    }
}