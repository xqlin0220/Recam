namespace Remp.Service.DTOs
{
    public class PublishListcaseResultDto
    {
        public int ListcaseId { get; set; }
        public string ShareToken { get; set; } = string.Empty;
        public string ShareableUrl { get; set; } = string.Empty;
    }
}