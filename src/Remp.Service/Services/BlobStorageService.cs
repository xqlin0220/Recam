using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".tif", ".tiff", ".heic", ".heif"
        };

        private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4", ".mov", ".avi", ".wmv", ".mkv", ".webm", ".m4v", ".3gp"
        };

        private static readonly HashSet<string> VrExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".glb", ".gltf", ".fbx", ".obj", ".usdz", ".stl"
        };

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"] ?? string.Empty;
            _containerName = configuration["AzureStorage:MediaContainer"] ?? "media";
        }

        public async Task<BlobUploadResultDto> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required.");
            }

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("File extension is missing.");
            }

            var category = ResolveCategory(extension);

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var safeOriginalName = Path.GetFileNameWithoutExtension(file.FileName);
            var normalizedName = string.Concat(safeOriginalName.Split(Path.GetInvalidFileNameChars()));
            var blobName = $"{category}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}_{normalizedName}{extension}";

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            var headers = new BlobHttpHeaders
            {
                ContentType = ResolveContentType(file.ContentType, extension)
            };

            var metadata = new Dictionary<string, string>
            {
                ["originalFileName"] = file.FileName,
                ["category"] = category,
                ["uploadedAtUtc"] = DateTime.UtcNow.ToString("O")
            };

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = headers,
                Metadata = metadata
            });

            var accessUrl = GenerateReadSasUrl(_containerName, blobName);

            return new BlobUploadResultDto
            {
                FileName = file.FileName,
                BlobName = blobName,
                ContainerName = _containerName,
                ContentType = headers.ContentType ?? file.ContentType,
                Size = file.Length,
                BlobUrl = blobClient.Uri.ToString(),
                AccessUrl = accessUrl,
                Category = category
            };
        }

        public string GenerateReadSasUrl(string containerName, string blobName, int expiryMinutes = 30)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobClient = blobServiceClient
                .GetBlobContainerClient(containerName)
                .GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        private static string ResolveCategory(string extension)
        {
            if (ImageExtensions.Contains(extension)) return "image";
            if (VideoExtensions.Contains(extension)) return "video";
            if (VrExtensions.Contains(extension)) return "vr";

            throw new ArgumentException($"Unsupported file extension: {extension}");
        }

        private static string ResolveContentType(string? uploadedContentType, string extension)
        {
            if (!string.IsNullOrWhiteSpace(uploadedContentType) &&
                !uploadedContentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            {
                return uploadedContentType;
            }

            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".tif" or ".tiff" => "image/tiff",
                ".heic" => "image/heic",
                ".heif" => "image/heif",

                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".wmv" => "video/x-ms-wmv",
                ".mkv" => "video/x-matroska",
                ".webm" => "video/webm",
                ".m4v" => "video/x-m4v",
                ".3gp" => "video/3gpp",

                ".glb" => "model/gltf-binary",
                ".gltf" => "model/gltf+json",
                ".fbx" => "application/octet-stream",
                ".obj" => "text/plain",
                ".usdz" => "model/vnd.usdz+zip",
                ".stl" => "model/stl",

                _ => "application/octet-stream"
            };
        }

        private static string GetFileNameFromBlobUrl(Uri blobUri)
        {
            return Uri.UnescapeDataString(Path.GetFileName(blobUri.AbsolutePath));
        }

        private static string GetAccountNameFromConnectionString(string connectionString)
        {
            return connectionString
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .Where(parts => parts.Length == 2)
                .First(parts => parts[0].Equals("AccountName", StringComparison.OrdinalIgnoreCase))[1];
        }

        private static string GetAccountKeyFromConnectionString(string connectionString)
        {
            return connectionString
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .Where(parts => parts.Length == 2)
                .First(parts => parts[0].Equals("AccountKey", StringComparison.OrdinalIgnoreCase))[1];
        }

        public async Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl))
            {
                throw new ArgumentException("Blob URL is required.");
            }

            if (!Uri.TryCreate(blobUrl, UriKind.Absolute, out var blobUri))
            {
                throw new ArgumentException("Invalid blob URL.");
            }

            var blobClient = new BlobClient(blobUri);

            var exists = await blobClient.ExistsAsync();
            if (!exists.Value)
            {
                throw new FileNotFoundException("Blob file not found.");
            }

            var properties = await blobClient.GetPropertiesAsync();
            var contentType = properties.Value.ContentType ?? "application/octet-stream";

            var fileName = Path.GetFileName(blobUri.LocalPath);

            if (properties.Value.Metadata.TryGetValue("originalFileName", out var originalFileName) &&
                !string.IsNullOrWhiteSpace(originalFileName))
            {
                fileName = originalFileName;
            }

            var stream = await blobClient.OpenReadAsync();

            return (stream, contentType, fileName);
        }
    }
}