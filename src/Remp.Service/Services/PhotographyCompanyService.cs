using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class PhotographyCompanyService : IPhotographyCompanyService
{
    private readonly AppDbContext _db;

    public PhotographyCompanyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAgentAsync(string photographyCompanyUserId, string agentId)
    {
        if (string.IsNullOrWhiteSpace(agentId))
            throw new ArgumentException("AgentId is required.");

        // 1) Validate agent exists
        var agentExists = await _db.Agents.AsNoTracking().AnyAsync(a => a.Id == agentId);
        if (!agentExists)
            throw new KeyNotFoundException("Agent not found.");

        // 2) Validate photography company exists
        // NOTE: depends on your schema. If PhotographyCompany has UserId field, adjust accordingly.
        var companyExists = await _db.PhotographyCompanies.AsNoTracking()
            .AnyAsync(pc => pc.Id == photographyCompanyUserId);
        if (!companyExists)
            throw new KeyNotFoundException("Photography company not found.");

        // 3) Check relation exists
        var exists = await _db.AgentPhotographyCompanies.AnyAsync(x =>
            x.AgentId == agentId &&
            x.PhotographyCompanyId == photographyCompanyUserId);

        if (exists)
            throw new InvalidOperationException("Agent already belongs to this photography company.");

        // 4) Insert joint row
        var join = new AgentPhotographyCompany
        {
            AgentId = agentId,
            PhotographyCompanyId = photographyCompanyUserId
        };

        _db.AgentPhotographyCompanies.Add(join);
        await _db.SaveChangesAsync();
    }
}