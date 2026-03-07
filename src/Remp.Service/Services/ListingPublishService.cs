using Microsoft.Extensions.Configuration;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using System.Text.RegularExpressions;

namespace Remp.Service.Services
{
    public class ListingPublishService : IListingPublishService
    {
        private readonly IListcaseRepository _listcaseRepository;
        private readonly string _baseUrl;

        private const string TokenPrefix = "[[SHARE_TOKEN:";
        private const string TokenSuffix = "]]";

        public ListingPublishService(
            IListcaseRepository listcaseRepository,
            IConfiguration configuration)
        {
            _listcaseRepository = listcaseRepository;
            _baseUrl = configuration["ShareableLink:BaseUrl"] ?? "";
        }

        public async Task<PublishListcaseResultDto> PublishAsync(int listcaseId)
        {
            if (listcaseId <= 0)
                throw new ArgumentException("Invalid listcaseId.");

            var listcase = await _listcaseRepository.GetByIdAsync(listcaseId);
            if (listcase == null)
                throw new KeyNotFoundException($"Listcase {listcaseId} not found.");

            var token = ExtractToken(listcase.Description);

            if (string.IsNullOrWhiteSpace(token))
            {
                token = Guid.NewGuid().ToString("N");

                var marker = $"{TokenPrefix}{token}{TokenSuffix}";

                if (string.IsNullOrWhiteSpace(listcase.Description))
                    listcase.Description = marker;
                else
                    listcase.Description += $" {marker}";

                await _listcaseRepository.UpdateAsync(listcase);
            }

            var shareUrl = $"{_baseUrl.TrimEnd('/')}/property/{token}";

            return new PublishListcaseResultDto
            {
                ListcaseId = listcase.Id,
                ShareToken = token,
                ShareableUrl = shareUrl
            };
        }

        private static string? ExtractToken(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return null;

            var match = Regex.Match(description, @"\[\[SHARE_TOKEN:(.*?)\]\]");
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}