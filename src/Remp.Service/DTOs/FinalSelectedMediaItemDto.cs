namespace Remp.Service.DTOs
{
    public class FinalSelectedMediaItemDto
    {
        public int SelectedMediaId { get; set; }
        public int MediaAssetId { get; set; }
        public int MediaType { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public bool IsHero { get; set; }
        public bool IsSelect { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}