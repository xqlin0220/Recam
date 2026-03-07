using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Models.Enums;

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

        public async Task<bool> ListcaseExistsAsync(int listcaseId)
        {
            return await _context.Listcases.AnyAsync(x => x.Id == listcaseId);
        }

        public async Task<MediaAsset?> GetByIdAsync(int mediaAssetId)
        {
            return await _context.MediaAssets
                .FirstOrDefaultAsync(x => x.Id == mediaAssetId && !x.IsDeleted);
        }
        public async Task<List<MediaAsset>> GetByListcaseIdAsync(int listcaseId)
        {
            return await _context.MediaAssets
                .Where(x => x.ListcaseId == listcaseId && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<MediaAsset?> GetHeroByListcaseIdAsync(int listcaseId)
        {
            return await _context.MediaAssets
                .FirstOrDefaultAsync(x => x.ListcaseId == listcaseId
                    && x.IsHero
                    && !x.IsDeleted);
        }

        public async Task UpdateAsync(MediaAsset mediaAsset)
        {
            _context.MediaAssets.Update(mediaAsset);
            await _context.SaveChangesAsync();
        }
        public async Task<List<MediaAsset>> GetFinalSelectedByListcaseIdAsync(int listcaseId)
        {
            return await _context.MediaAssets
                .Where(x => x.ListcaseId == listcaseId
                            && x.IsSelect
                            && !x.IsDeleted)
                .OrderByDescending(x => x.IsHero)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }
        public async Task<List<MediaAsset>> GetByIdsAsync(List<int> mediaIds)
        {
            return await _context.MediaAssets
                .Where(x => mediaIds.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<MediaAsset>> GetPicturesByListcaseIdAsync(int listcaseId)
        {
            return await _context.MediaAssets
                .Where(x => x.ListcaseId == listcaseId
                    && x.MediaType == MediaType.Picture
                    && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(List<MediaAsset> mediaAssets)
        {
            _context.MediaAssets.UpdateRange(mediaAssets);
            await _context.SaveChangesAsync();
        }
    }
}