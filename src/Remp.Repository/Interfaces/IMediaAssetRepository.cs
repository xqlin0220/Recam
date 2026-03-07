using Remp.Models.Entities;

namespace Remp.Repository.Interfaces
{
    public interface IMediaAssetRepository
    {
        Task<MediaAsset> AddAsync(MediaAsset mediaAsset);
        Task<bool> ListcaseExistsAsync(int listcaseId);
        Task<MediaAsset?> GetByIdAsync(int mediaAssetId);
        Task<List<MediaAsset>> GetByListcaseIdAsync(int listcaseId);
        Task<MediaAsset?> GetHeroByListcaseIdAsync(int listcaseId);
        Task UpdateAsync(MediaAsset mediaAsset);
    }
}