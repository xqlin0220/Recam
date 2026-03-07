using Microsoft.AspNetCore.Http;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.IO.Compression;

namespace Remp.Service.Services
{
    public class MediaAssetService : IMediaAssetService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMediaAssetRepository _mediaAssetRepository;

        public MediaAssetService(
            IBlobStorageService blobStorageService,
            IMediaAssetRepository mediaAssetRepository)
        {
            _blobStorageService = blobStorageService;
            _mediaAssetRepository = mediaAssetRepository;
        }

        public async Task<List<MediaAssetUploadResultDto>> UploadMediaAssetsAsync(
            UploadMediaAssetsRequestDto request,
            string userId)
        {
            ValidateRequest(request);

            var listingCaseExists = await _mediaAssetRepository.ListingCaseExistsAsync(request.ListingCaseId);
            if (!listingCaseExists)
            {
                throw new ArgumentException($"ListcaseId {request.ListingCaseId} does not exist.");
            }

            var results = new List<MediaAssetUploadResultDto>();

            foreach (var file in request.Files)
            {
                ValidateFileAgainstMediaType(file, request.Type);

                var uploadResult = await _blobStorageService.UploadAsync(file);

                var mediaAsset = new MediaAsset
                {
                    MediaType = request.Type,
                    MediaUrl = uploadResult.BlobUrl, 
                    UploadedAt = DateTime.UtcNow,
                    IsSelect = false,
                    IsHero = false,
                    IsDeleted = false,
                    ListcaseId = request.ListingCaseId,
                    UserId = userId
                };

                var savedEntity = await _mediaAssetRepository.AddAsync(mediaAsset);

                results.Add(new MediaAssetUploadResultDto
                {
                    MediaAssetId = savedEntity.Id,
                    ListingCaseId = savedEntity.ListcaseId,
                    MediaType = (int)savedEntity.MediaType,
                    MediaUrl = savedEntity.MediaUrl,
                    UploadedAt = savedEntity.UploadedAt
                });
            }

            return results;
        }

        private static void ValidateRequest(UploadMediaAssetsRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException("Request is required.");
            }

            if (request.ListingCaseId <= 0)
            {
                throw new ArgumentException("ListingCaseId is required.");
            }

            if (request.Files == null || request.Files.Count == 0)
            {
                throw new ArgumentException("At least one file is required.");
            }

            if (!Enum.IsDefined(typeof(MediaType), request.Type))
            {
                throw new ArgumentException("Invalid media type.");
            }

            if (request.Type != MediaType.Picture && request.Files.Count > 1)
            {
                throw new ArgumentException("Only picture type allows multiple file upload.");
            }
        }

        public async Task<(Stream Content, string ContentType, string FileName)> DownloadMediaAssetAsync(int mediaAssetId)
        {
            if (mediaAssetId <= 0)
            {
                throw new ArgumentException("Invalid mediaAssetId.");
            }

            var mediaAsset = await _mediaAssetRepository.GetByIdAsync(mediaAssetId);
            if (mediaAsset == null)
            {
                throw new KeyNotFoundException($"MediaAsset {mediaAssetId} not found.");
            }

            if (string.IsNullOrWhiteSpace(mediaAsset.MediaUrl))
            {
                throw new ArgumentException("Media URL is empty.");
            }

            return await _blobStorageService.DownloadFileAsync(mediaAsset.MediaUrl);
        }

        public async Task<(Stream Content, string ContentType, string FileName)> DownloadListcaseZipAsync(int listingCaseId)
        {
            if (listingCaseId <= 0)
            {
                throw new ArgumentException("Invalid listingCaseId.");
            }

            var listingCaseExists = await _mediaAssetRepository.ListingCaseExistsAsync(listingCaseId);
            if (!listingCaseExists)
            {
                throw new KeyNotFoundException($"ListingCase {listingCaseId} not found.");
            }

            var mediaAssets = await _mediaAssetRepository.GetByListingCaseIdAsync(listingCaseId);
            if (mediaAssets.Count == 0)
            {
                throw new KeyNotFoundException($"No media assets found for ListingCase {listingCaseId}.");
            }

            var zipStream = new MemoryStream();

            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var mediaAsset in mediaAssets)
                {
                    if (string.IsNullOrWhiteSpace(mediaAsset.MediaUrl))
                    {
                        continue;
                    }

                    var (fileStream, _, fileName) = await _blobStorageService.DownloadFileAsync(mediaAsset.MediaUrl);

                    await using (fileStream)
                    {
                        var safeFileName = BuildUniqueEntryName(
                            usedNames,
                            BuildEntryName(mediaAsset, fileName));

                        var entry = archive.CreateEntry(safeFileName, CompressionLevel.Fastest);

                        await using var entryStream = entry.Open();
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }

            zipStream.Position = 0;

            var zipFileName = $"listingcase-{listingCaseId}-media.zip";
            return (zipStream, "application/zip", zipFileName);
        }

        private static string BuildEntryName(MediaAsset mediaAsset, string originalFileName)
        {
            var folder = mediaAsset.MediaType switch
            {
                MediaType.Picture => "pictures",
                MediaType.Video => "videos",
                MediaType.FloorPlan => "floorplans",
                MediaType.VRTour => "vrtours",
                _ => "others"
            };

            var fileName = string.IsNullOrWhiteSpace(originalFileName)
                ? $"media-{mediaAsset.Id}"
                : originalFileName;

            fileName = SanitizeFileName(fileName);

            return $"{folder}/{fileName}";
        }

        private static string BuildUniqueEntryName(HashSet<string> usedNames, string entryName)
        {
            if (!usedNames.Contains(entryName))
            {
                usedNames.Add(entryName);
                return entryName;
            }

            var directory = Path.GetDirectoryName(entryName)?.Replace("\\", "/");
            var fileName = Path.GetFileNameWithoutExtension(entryName);
            var extension = Path.GetExtension(entryName);

            var counter = 1;
            string candidate;

            do
            {
                var newName = $"{fileName}_{counter}{extension}";
                candidate = string.IsNullOrWhiteSpace(directory)
                    ? newName
                    : $"{directory}/{newName}";
                counter++;
            }
            while (usedNames.Contains(candidate));

            usedNames.Add(candidate);
            return candidate;
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(fileName.Select(ch => invalidChars.Contains(ch) ? '_' : ch));
        }

        private static void ValidateFileAgainstMediaType(IFormFile file, MediaType type)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var pictureExtensions = new HashSet<string>
            {
                ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".heic", ".heif"
            };

            var videoExtensions = new HashSet<string>
            {
                ".mp4", ".mov", ".avi", ".wmv", ".mkv", ".webm", ".m4v", ".3gp"
            };

            var floorPlanExtensions = new HashSet<string>
            {
                ".jpg", ".jpeg", ".png", ".pdf", ".webp"
            };

            var vrTourExtensions = new HashSet<string>
            {
                ".glb", ".gltf", ".fbx", ".obj", ".usdz", ".stl"
            };

            var isValid = type switch
            {
                MediaType.Picture => pictureExtensions.Contains(extension),
                MediaType.Video => videoExtensions.Contains(extension),
                MediaType.FloorPlan => floorPlanExtensions.Contains(extension),
                MediaType.VRTour => vrTourExtensions.Contains(extension),
                _ => false
            };

            if (!isValid)
            {
                throw new ArgumentException($"File extension {extension} is not allowed for media type {type}.");
            }
        }

    }
}