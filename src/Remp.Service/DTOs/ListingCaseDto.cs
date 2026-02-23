using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ListingCaseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public ListcaseStatus ListingStatus { get; set; }
}