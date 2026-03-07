using Remp.Service.DTOs;

namespace Remp.Service.Interfaces
{
    public interface IMediaAssetService
    {
        Task<List<MediaAssetUploadResultDto>> UploadMediaAssetsAsync(
            UploadMediaAssetsRequestDto request,
            string userId);

        Task<(Stream Content, string ContentType, string FileName)> DownloadMediaAssetAsync(int mediaAssetId);
        Task<(Stream Content, string ContentType, string FileName)> DownloadListcaseZipAsync(int listcaseId);
    }
}