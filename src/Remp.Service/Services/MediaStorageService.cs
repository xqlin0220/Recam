using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Remp.Service.Interfaces;

namespace Remp.Service.Services
{
    public class MediaStorageService : IMediaStorageService
    {
        private readonly string _connectionString;
        private readonly string _imagesContainer;
        private readonly string _videosContainer;

        public MediaStorageService(IConfiguration config)
        {
            _connectionString = config["AzureStorage:ConnectionString"] ?? string.Empty;
            _imagesContainer = config["AzureStorage:ImagesContainer"] ?? "images";
            _videosContainer = config["AzureStorage:VideosContainer"] ?? "videos";
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string mediaType)
        {
            var containerName = mediaType.ToLower() == "image"
                ? _imagesContainer
                : _videosContainer;

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });

            return GenerateSasUrl(containerName, blobName);
        }

        private string GenerateSasUrl(string containerName, string blobName)
        {
            var blobClient = new BlobClient(_connectionString, containerName, blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }
    }
}