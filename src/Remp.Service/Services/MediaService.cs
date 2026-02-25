using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class MediaService : IMediaService
{
    private readonly AppDbContext _db;
    private readonly ICaseHistoryService _history;

    public MediaService(AppDbContext db, ICaseHistoryService history)
    {
        _db = db;
        _history = history;
    }

    public async Task DeleteAsync(int mediaId, string userId, string email, string role, string? ip, string? userAgent)
    {
        if (role != "photographyCompany")
            throw new UnauthorizedAccessException("Only photographyCompany can delete media files.");

        // Load media
        var media = await _db.MediaAssets.FirstOrDefaultAsync(m => m.Id == mediaId /* && !m.IsDeleted */);
        if (media == null)
            throw new ArgumentException($"Media {mediaId} not found.");

        // Verify ownership via Listcase.UserId
        var listcase = await _db.Listcases.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == media.ListcaseId && !l.IsDeleted);

        if (listcase == null)
            throw new ArgumentException("Listing case for this media not found.");

        if (listcase.UserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to delete this media.");

        // Snapshot for audit
        var snapshot = new
        {
            MediaId = mediaId,
            media.ListcaseId,
            media.MediaType,
            media.MediaUrl
        };

        // Hard delete record from SQL Server
        _db.MediaAssets.Remove(media);
        await _db.SaveChangesAsync();

        // Mongo audit
        await _history.LogMediaDeletedAsync(listcase.Id, mediaId, userId, email, role, ip, userAgent, snapshot);
    }
}