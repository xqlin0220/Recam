using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ListcaseDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;

    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public int Postcode { get; set; }

    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }

    public double Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }
    public double FloorArea { get; set; }

    public PropertyType PropertyType { get; set; }
    public SaleCategory SaleCategory { get; set; }
    public ListcaseStatus ListcaseStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<MediaAssetDto> MediaAssets { get; set; } = new();
    public List<AgentDto> Agents { get; set; } = new();
}

public class MediaAssetDto
{
    public int Id { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = default!;
    public DateTime UploadedAt { get; set; }
    public bool IsSelect { get; set; }
    public bool IsHero { get; set; }
}

public class AgentDto
{
    public string Id { get; set; } = default!;
    public string AgentFirstName { get; set; } = default!;
    public string AgentLastName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public string CompanyName { get; set; } = default!;
}