namespace Remp.Service.DTOs;

public class CaseContactDto
{
    public int ContactId { get; set; }
    public int ListcaseId { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
}