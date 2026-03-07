namespace Remp.Service.DTOs
{
    public class MediaAssetUploadResultDto
    {
        public int MediaAssetId { get; set; }
        public int ListingCaseId { get; set; }
        public int MediaType { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}