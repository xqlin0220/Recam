using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly IListingCaseService _listingService;

    public ListingsController(IListingCaseService listingService)
    {
        _listingService = listingService;
    }

    // only photographyCompany can create
    [HttpPost]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<ListingCaseDto>>> Create([FromBody] CreateListingCaseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue(ClaimTypes.Name) 
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? "";

        // better: use JwtRegisteredClaimNames.Sub, but we put sub in token
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!string.IsNullOrWhiteSpace(sub)) userId = sub;

        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _listingService.CreateAsync(request, userId, email, role, ip, ua);
        return Ok(ApiResponse<ListingCaseDto>.Ok(result, "ListingCase created."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ListingCaseDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        var result = await _listingService.GetAllAsync(userId, role, new PagingQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return Ok(ApiResponse<PagedResult<ListingCaseDto>>.Ok(result));
    }
}