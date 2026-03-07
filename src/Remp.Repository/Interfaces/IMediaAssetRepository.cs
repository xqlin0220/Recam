using Remp.Models.Entities;

namespace Remp.Repository.Interfaces
{
    public interface IMediaAssetRepository
    {
        Task<MediaAsset> AddAsync(MediaAsset mediaAsset);
        Task<bool> ListingCaseExistsAsync(int listingCaseId);
    }
}