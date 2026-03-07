namespace Remp.Service.DTOs
{
    public class GetFinalSelectedMediaResponseDto
    {
        public int ListingId { get; set; }
        public string ListingStatus { get; set; } = string.Empty;
        public List<FinalSelectedMediaItemDto> MediaItems { get; set; } = new();
    }
}