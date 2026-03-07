using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services
{
    public class SelectedMediaService : ISelectedMediaService
    {
        private readonly IListcaseRepository _listcaseRepository;
        private readonly IMediaAssetRepository _mediaAssetRepository;

        public SelectedMediaService(
            IListcaseRepository listcaseRepository,
            IMediaAssetRepository mediaAssetRepository)
        {
            _listcaseRepository = listcaseRepository;
            _mediaAssetRepository = mediaAssetRepository;
        }

        public async Task<GetFinalSelectedMediaResponseDto> GetFinalSelectedMediaAsync(int listingId)
        {
            if (listingId <= 0)
            {
                throw new ArgumentException("Invalid listing id.");
            }

            var listing = await _listcaseRepository.GetByIdAsync(listingId);
            if (listing == null)
            {
                throw new KeyNotFoundException($"Listing {listingId} not found.");
            }

            var listingStatus = listing.ListcaseStatus.ToString();

            ValidateListingStatusForFinalSelection(listingStatus);

            var mediaAssets = await _mediaAssetRepository.GetFinalSelectedByListcaseIdAsync(listingId);

            return new GetFinalSelectedMediaResponseDto
            {
                ListingId = listingId,
                ListingStatus = listingStatus,
                MediaItems = mediaAssets.Select(x => new FinalSelectedMediaItemDto
                {
                    MediaAssetId = x.Id,
                    MediaType = (int)x.MediaType,
                    MediaUrl = x.MediaUrl,
                    IsHero = x.IsHero,
                    IsSelect = x.IsSelect,
                    UploadedAt = x.UploadedAt
                }).ToList()
            };
        }

        private static void ValidateListingStatusForFinalSelection(string listingStatus)
        {
            var allowedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Completed",
                "Approved",
                "Delivered",
                "FinalSelectionReady"
            };

            if (!allowedStatuses.Contains(listingStatus))
            {
                throw new ArgumentException(
                    $"Listing current status '{listingStatus}' does not allow final selection retrieval.");
            }
        }
    }
}