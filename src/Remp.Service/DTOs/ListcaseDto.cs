using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ListcaseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public int Postcode { get; set; }

    public PropertyType PropertyType { get; set; }
    public ListcaseStatus ListcaseStatus { get; set; }

    public DateTime CreatedAt { get; set; }
}