using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/photography-companies")]
public class PhotographyCompaniesController : ControllerBase
{
    private readonly IPhotographyCompanyService _service;

    public PhotographyCompaniesController(IPhotographyCompanyService service)
    {
        _service = service;
    }

    [HttpPost("agents")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<object>>> AddAgent(
        [FromBody] AddAgentToPhotographyCompanyRequest request)
    {
        var photographyCompanyUserId =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "";

        await _service.AddAgentAsync(photographyCompanyUserId, request.AgentId);

        return Ok(ApiResponse<object>.Ok(new { }, "Agent added successfully."));
    }

    [HttpGet("agents")]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<List<AgentSummaryDto>>>> GetAgents()
    {
        var companyUserId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "";
        if (string.IsNullOrWhiteSpace(companyUserId))
            return Unauthorized(ApiResponse<List<AgentSummaryDto>>.Fail("Invalid token."));

        var result = await _service.GetAgentsAsync(companyUserId);
        return Ok(ApiResponse<List<AgentSummaryDto>>.Ok(result));
    }
}