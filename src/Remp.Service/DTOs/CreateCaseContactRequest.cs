namespace Remp.Service.DTOs;

public class CreateCaseContactRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public string? CompanyName { get; set; }
    public string? ProfileUrl { get; set; }
}