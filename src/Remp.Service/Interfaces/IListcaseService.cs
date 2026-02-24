using Remp.Service.DTOs;
using Remp.Models.Enums;

namespace Remp.Service.Interfaces;

public interface IListcaseService
{
    Task<ListcaseDto> CreateAsync(CreateListcaseRequest request, string userId, string email, string role, string? ip, string? userAgent);
    Task<PagedResult<ListcaseDto>> GetAllAsync(
        string userId,
        string role,
        PagingQuery query);
    Task<ListcaseDto> UpdateAsync(
        int id,
        UpdateListcaseRequest request,
        string userId,
        string email,
        string role,
        string? ip,
        string? userAgent);

    Task DeleteAsync(int id, string userId, string email, string role, string? ip, string? userAgent);
    Task<ListcaseDetailDto> GetDetailAsync(int id, string userId, string role);
    Task ChangeStatusAsync(
        int id,
        ListcaseStatus newStatus,
        string userId,
        string email,
        string role,
        string? ip,
        string? userAgent);
}