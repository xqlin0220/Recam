using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Utilities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Remp.API.Controllers;

[ApiController]
[Route("api/agents")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentsController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost]
    [Authorize(Roles = "photographyCompany")]
    public async Task<ActionResult<ApiResponse<CreateAgentResponse>>> CreateAgent([FromBody] CreateAgentRequest request)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "";
        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? "";
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _agentService.CreateAgentAsync(request, userId, email, role, ip, ua);
        return Ok(ApiResponse<CreateAgentResponse>.Ok(result, "Agent account created and email sent."));
    }
}