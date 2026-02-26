using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/listings")]
public class ListController : ControllerBase
{
    private readonly IListcaseService _listService;

    public ListController(IListcaseService listingService)
    {
        _listService = listingService;
    }

    // only photographyCompany can create
    [HttpPost]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<ListcaseDto>>> Create([FromBody] CreateListcaseRequest request)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _listService.CreateAsync(request, userId, email, role, ip, ua);
        return Ok(ApiResponse<ListcaseDto>.Ok(result, "Listcase created."));
    }

    [HttpGet]
    [Authorize(Roles = "photographyCompany,user")]
    public async Task<ActionResult<ApiResponse<PagedResult<ListcaseDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        var result = await _listService.GetAllAsync(userId, role, new PagingQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return Ok(ApiResponse<PagedResult<ListcaseDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "photographyCompany,user")]
    public async Task<ActionResult<ApiResponse<ListcaseDetailDto>>> GetDetail(int id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        var result = await _listService.GetDetailAsync(id, userId, role);
        return Ok(ApiResponse<ListcaseDetailDto>.Ok(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<ListcaseDto>>> Update(int id, [FromBody] UpdateListcaseRequest request)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _listService.UpdateAsync(id, request, userId, email, role, ip, ua);
        return Ok(ApiResponse<ListcaseDto>.Ok(result, "Listcase updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        await _listService.DeleteAsync(id, userId, email, role, ip, ua);

        return Ok(ApiResponse<object>.Ok(new { id }, $"Listing {id} deleted successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(int id, [FromBody] ChangeStatusRequest request)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "photographyCompany";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        await _listService.ChangeStatusAsync(id, request.NewStatus, userId, email, role, ip, ua);

        return Ok(ApiResponse<object>.Ok(new
        {
            id,
            status = request.NewStatus.ToString()
        }, "Listing status updated."));
    }

    [HttpGet("{id:int}/media")]
    [Authorize(Roles = "photographyCompany,user")]
    public async Task<ActionResult<ApiResponse<List<ListMediaGroupDto>>>> GetMedia(int id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        var result = await _listService.GetMediaGroupedAsync(id, userId, role);
        return Ok(ApiResponse<List<ListMediaGroupDto>>.Ok(result));
    }


    [HttpGet("{id:int}/contacts")]
    [Authorize(Roles = "photographyCompany,user")]
    public async Task<ActionResult<ApiResponse<List<CaseContactDto>>>> GetContacts(int id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

        var result = await _listService.GetCaseContactsAsync(id, userId, role);
        return Ok(ApiResponse<List<CaseContactDto>>.Ok(result));
    }
}