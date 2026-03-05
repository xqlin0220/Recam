using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.Security.Cryptography;

namespace Remp.Service.Services;

public class AgentService : IAgentService
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IAdminAuditLogService _audit;

    public AgentService(
        AppDbContext db,
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        IAdminAuditLogService audit)
    {
        _db = db;
        _userManager = userManager;
        _emailSender = emailSender;
        _audit = audit;
    }

    public async Task<CreateAgentResponse> CreateAgentAsync(
        CreateAgentRequest request,
        string photographyCompanyId,
        string performedByEmail,
        string role,
        string? ip,
        string? userAgent)
    {
        if (role != "photographyCompany")
            throw new UnauthorizedAccessException("Only photographyCompany can create agent accounts.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(request.CompanyName))
            throw new ArgumentException("CompanyName is required.");

        var email = request.Email.Trim().ToLowerInvariant();

        // Ensure photography company exists
        var companyExists = await _db.PhotographyCompanies
            .AsNoTracking()
            .AnyAsync(pc => pc.Id == photographyCompanyId);
        if (!companyExists)
            throw new KeyNotFoundException("Photography company not found.");

        // Ensure email not used
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
            throw new InvalidOperationException("Email already exists.");

        // Generate random password
        var password = GeneratePassword(12);

        // Create Identity user
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var msg = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new ArgumentException($"Create user failed: {msg}");
        }

        // Assign agent role
        var roleResult = await _userManager.AddToRoleAsync(user, "user");
        if (!roleResult.Succeeded)
        {
            var msg = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            throw new ArgumentException($"Assign role failed: {msg}");
        }

        // Create Agent entity (Id = Identity userId)
        var agent = new Agent
        {
            Id = user.Id,
            AgentFirstName = request.AgentFirstName?.Trim() ?? "",
            AgentLastName = request.AgentLastName?.Trim() ?? "",
            CompanyName = request.CompanyName.Trim(),
            AvatarUrl = null
        };
        _db.Agents.Add(agent);

        // Create joint relation AgentPhotographyCompany
        var relationExists = await _db.AgentPhotographyCompanies.AnyAsync(x =>
            x.AgentId == user.Id && x.PhotographyCompanyId == photographyCompanyId);

        if (!relationExists)
        {
            _db.AgentPhotographyCompanies.Add(new AgentPhotographyCompany
            {
                AgentId = user.Id,
                PhotographyCompanyId = photographyCompanyId
            });
        }

        await _db.SaveChangesAsync();

        // Send login info email (use your existing SendEmailAsync to avoid interface mismatch)
        var subject = "Your agent account has been created";
        var body =
$@"Hello {agent.AgentFirstName} {agent.AgentLastName},

Your agent account is ready.

Login Email: {email}
Temporary Password: {password}

Please log in and change your password as soon as possible.";

        await _emailSender.SendEmailAsync(email, subject, body);

        // Mongo audit
        await _audit.LogAgentCreatedAsync(photographyCompanyId, performedByEmail, user.Id, email, ip, userAgent);

        return new CreateAgentResponse
        {
            AgentUserId = user.Id,
            Email = email
        };
    }

    private static string GeneratePassword(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@$?*";
        Span<char> buffer = stackalloc char[length];
        for (int i = 0; i < length; i++)
            buffer[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        return new string(buffer);
    }
}