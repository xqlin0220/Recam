using Remp.Models.Enums;

namespace Remp.Models.Entities;

public class MediaAsset
{
    public int Id { get; set; }   // PK

    public MediaType MediaType { get; set; }

    public string MediaUrl { get; set; } = default!;

    public DateTime UploadedAt { get; set; }

    public bool IsSelect { get; set; }

    public bool IsHero { get; set; }

    public bool IsDeleted { get; set; }

    // FK
    public int ListcaseId { get; set; }

    public string UserId { get; set; } = default!;

    // Navigation
    public Listcase Listcase { get; set; } = default!;
}