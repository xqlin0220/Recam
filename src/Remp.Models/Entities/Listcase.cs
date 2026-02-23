using Remp.Models.Enums;

namespace Remp.Models.Entities;

public class Listcase
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

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

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public PropertyType PropertyType { get; set; }

    public SaleCategory SaleCategory { get; set; }
    public ListcaseStatus ListcaseStatus { get; set; }

    public string UserId { get; set; } = default!;

    public ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();

    public ICollection<CaseContact> CaseContacts { get; set; } = new List<CaseContact>();

    public ICollection<AgentListcase> AgentListcases { get; set; } = new List<AgentListcase>();
}