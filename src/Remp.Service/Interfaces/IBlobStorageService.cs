using Microsoft.AspNetCore.Http;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces
{
    public interface IBlobStorageService
    {
        Task<BlobUploadResultDto> UploadAsync(IFormFile file);
        string GenerateReadSasUrl(string containerName, string blobName, int expiryMinutes = 30);
        Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl);
    }
}