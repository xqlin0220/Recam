namespace Remp.Service.DTOs
{
    public class BlobUploadResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string BlobName { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string BlobUrl { get; set; } = string.Empty;
        public string AccessUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}