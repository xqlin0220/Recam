namespace Recam.Domain.Entities;

public class ListingCase
{
    public int Id { get; set; }   // PK

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

    public ListingStatus ListingStatus { get; set; }

    // FK to User (string)
    public string UserId { get; set; } = default!;

    // Navigation
    public ICollection<AgentListingCase> AgentListingCases { get; set; }
        = new List<AgentListingCase>();
}