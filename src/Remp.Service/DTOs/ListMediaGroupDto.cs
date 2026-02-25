using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ListMediaGroupDto
{
    public MediaType MediaType { get; set; }
    public List<ListingMediaItemDto> Items { get; set; } = new();
}

public class ListingMediaItemDto
{
    public int Id { get; set; }
    public string MediaUrl { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
    public bool IsSelect { get; set; }
    public bool IsHero { get; set; }
}