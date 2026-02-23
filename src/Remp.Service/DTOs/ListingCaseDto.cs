using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ListingCaseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public int Postcode { get; set; }

    public PropertyType PropertyType { get; set; }
    public ListcaseStatus ListingStatus { get; set; }

    public DateTime CreatedAt { get; set; }
}