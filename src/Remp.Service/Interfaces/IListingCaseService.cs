using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<ListingCaseDto> CreateAsync(CreateListingCaseRequest request, string userId, string email, string role, string? ip, string? userAgent);
    Task<PagedResult<ListingCaseDto>> GetAllAsync(
        string userId,
        string role,
        PagingQuery query);
}