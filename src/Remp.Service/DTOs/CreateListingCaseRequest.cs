using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class CreateListingCaseRequest
{
    public string Title { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public int Postcode { get; set; }

    public PropertyType PropertyType { get; set; }
    public SaleCategory SaleCategory { get; set; }

    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }

    public double Price { get; set; }
    public double FloorArea { get; set; }

    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
}