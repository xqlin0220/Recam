using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ListingCaseService : IListingCaseService
{
    private readonly AppDbContext _db;
    private readonly ICaseHistoryService _history;

    public ListingCaseService(AppDbContext db, ICaseHistoryService history)
    {
        _db = db;
        _history = history;
    }

    public async Task<ListingCaseDto> CreateAsync(
        CreateListingCaseRequest request,
        string userId,
        string email,
        string role,
        string? ip,
        string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title must not be empty.");

        if (request.Title.Length > 255)
            throw new ArgumentException("Title must be 255 characters or less.");

        if (string.IsNullOrWhiteSpace(request.Street) ||
            string.IsNullOrWhiteSpace(request.City) ||
            string.IsNullOrWhiteSpace(request.State))
            throw new ArgumentException("Address fields (street/city/state) are required.");

        if (request.Bedrooms < 0 || request.Bathrooms < 0 || request.Garages < 0)
            throw new ArgumentException("Bedrooms/Bathrooms/Garages must be non-negative.");

        if (request.Price < 0)
            throw new ArgumentException("Price must be non-negative.");

        var entity = new ListingCase
        {
            Title = request.Title.Trim(),
            Street = request.Street.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            Postcode = request.Postcode,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            Price = request.Price,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Garages = request.Garages,
            FloorArea = request.FloorArea,
            PropertyType = request.PropertyType,
            SaleCategory = request.SaleCategory,
            ListcaseStatus = ListcaseStatus.Created,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId
        };

        _db.ListingCases.Add(entity);
        await _db.SaveChangesAsync();

        await _history.LogCaseCreatedAsync(
            entity.Id,
            userId,
            email,
            role,
            ip,
            userAgent,
            snapshot: new
            {
                entity.Title,
                entity.Street,
                entity.City,
                entity.State,
                entity.Postcode,
                entity.PropertyType,
                entity.SaleCategory,
                entity.Bedrooms,
                entity.Bathrooms,
                entity.Garages,
                entity.Price
            });

        return new ListingCaseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            ListingStatus = entity.ListcaseStatus
        };
    }

    public async Task<PagedResult<ListingCaseDto>> GetAllAsync(string userId, string role, PagingQuery query)
    {
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        if (pageSize > 100) pageSize = 100;

        IQueryable<ListingCase> baseQuery;

        if (role == "photographyCompany")
        {
            baseQuery = _db.ListingCases.Where(x => x.UserId == userId && !x.IsDeleted);
        }
        else if (role == "user")
        {
            baseQuery =
                _db.AgentListingCases
                   .Where(al => al.AgentId == userId)
                   .Select(al => al.ListingCase)
                   .Where(lc => !lc.IsDeleted);
        }
        else
        {
            baseQuery = _db.ListingCases.Where(_ => false);
        }

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ListingCaseDto
            {
                Id = x.Id,
                Title = x.Title,
                Street = x.Street,
                City = x.City,
                State = x.State,
                Postcode = x.Postcode,
                PropertyType = x.PropertyType,
                ListingStatus = x.ListcaseStatus,  
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<ListingCaseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}