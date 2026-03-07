using Microsoft.AspNetCore.Http;
using Remp.Models.Enums;

namespace Remp.Service.DTOs
{
    public class UploadMediaAssetsRequestDto
    {
        public List<IFormFile> Files { get; set; } = new();
        public MediaType Type { get; set; }
        public int ListcaseId { get; set; }
    }
}