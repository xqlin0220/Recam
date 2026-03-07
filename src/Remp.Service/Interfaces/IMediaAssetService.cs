using Remp.Service.DTOs;

namespace Remp.Service.Interfaces
{
    public interface IMediaAssetService
    {
        Task<List<MediaAssetUploadResultDto>> UploadMediaAssetsAsync(
            UploadMediaAssetsRequestDto request,
            string userId);
    }
}