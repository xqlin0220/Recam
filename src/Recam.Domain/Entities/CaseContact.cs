namespace Recam.Domain.Entities;

public class CaseContact
{
    public int ContactId { get; set; }   // PK

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string? CompanyName { get; set; }

    public string? ProfileUrl { get; set; }

    public string Email { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;

    // FK
    public int ListingCaseId { get; set; }

    // Navigation
    public ListingCase ListingCase { get; set; } = default!;
}