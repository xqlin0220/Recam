using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListController : ControllerBase
{
    private readonly IListcaseService _listingService;

    public ListController(IListcaseService listingService)
    {
        _listingService = listingService;
    }

    // only photographyCompany can create
    [HttpPost]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<ListcaseDto>>> Create([FromBody] CreateListcaseRequest request)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _listingService.CreateAsync(request, userId, email, role, ip, ua);
        return Ok(ApiResponse<ListcaseDto>.Ok(result, "Listcase created."));
    }

    [HttpGet]
    [Authorize(Roles = "photographyCompany,user")]
    public async Task<ActionResult<ApiResponse<PagedResult<ListcaseDto>>>> GetAll(
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

        return Ok(ApiResponse<PagedResult<ListcaseDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<ListcaseDto>>> Update(int id, [FromBody] UpdateListcaseRequest request)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _listingService.UpdateAsync(id, request, userId, email, role, ip, ua);
        return Ok(ApiResponse<ListcaseDto>.Ok(result, "Listcase updated."));
    }
}