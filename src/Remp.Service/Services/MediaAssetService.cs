using Microsoft.AspNetCore.Http;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

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
                    MediaUrl = uploadResult.AccessUrl, 
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