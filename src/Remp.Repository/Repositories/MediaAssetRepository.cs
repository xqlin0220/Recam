using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories
{
    public class MediaAssetRepository : IMediaAssetRepository
    {
        private readonly AppDbContext _context;

        public MediaAssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MediaAsset> AddAsync(MediaAsset mediaAsset)
        {
            _context.MediaAssets.Add(mediaAsset);
            await _context.SaveChangesAsync();
            return mediaAsset;
        }

        public async Task<bool> ListingCaseExistsAsync(int listingCaseId)
        {
            return await _context.Listcases.AnyAsync(x => x.Id == listingCaseId);
        }

        public async Task<MediaAsset?> GetByIdAsync(int mediaAssetId)
        {
            return await _context.MediaAssets
                .FirstOrDefaultAsync(x => x.Id == mediaAssetId && !x.IsDeleted);
        }
        public async Task<List<MediaAsset>> GetByListingCaseIdAsync(int listingCaseId)
        {
            return await _context.MediaAssets
                .Where(x => x.ListcaseId == listingCaseId && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }
    }
}