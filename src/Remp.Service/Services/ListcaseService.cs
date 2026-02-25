using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class ListcaseService : IListcaseService
{
    private readonly AppDbContext _db;
    private readonly ICaseHistoryService _history;

    public ListcaseService(AppDbContext db, ICaseHistoryService history)
    {
        _db = db;
        _history = history;
    }

    public async Task<ListcaseDto> CreateAsync(
        CreateListcaseRequest request,
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

        var entity = new Listcase
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

        _db.Listcases.Add(entity);
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

        return new ListcaseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            ListcaseStatus = entity.ListcaseStatus
        };
    }

    public async Task<PagedResult<ListcaseDto>> GetAllAsync(string userId, string role, PagingQuery query)
    {
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        if (pageSize > 100) pageSize = 100;

        IQueryable<Listcase> baseQuery;

        if (role == "photographyCompany")
        {
            baseQuery = _db.Listcases.Where(x => x.UserId == userId && !x.IsDeleted);
        }
        else if (role == "user")
        {
            baseQuery =
                _db.AgentListcases
                   .Where(al => al.AgentId == userId)
                   .Select(al => al.Listcase)
                   .Where(lc => !lc.IsDeleted);
        }
        else
        {
            baseQuery = _db.Listcases.Where(_ => false);
        }

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ListcaseDto
            {
                Id = x.Id,
                Title = x.Title,
                Street = x.Street,
                City = x.City,
                State = x.State,
                Postcode = x.Postcode,
                PropertyType = x.PropertyType,
                ListcaseStatus = x.ListcaseStatus,  
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<ListcaseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<ListcaseDto> UpdateAsync(
        int id,
        UpdateListcaseRequest request,
        string userId,
        string email,
        string role,
        string? ip,
        string? userAgent)
    {
        // Business rule: only photographyCompany can update
        if (role != "photographyCompany")
            throw new UnauthorizedAccessException("Only photographyCompany users can update listing cases.");

        // Load listing
        var entity = await _db.Listcases.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
            throw new KeyNotFoundException($"Listcase with id {id} was not found.");

        // Business rule: can only update cases created under this account
        if (entity.UserId != userId)
            throw new UnauthorizedAccessException("You can only update listing cases created under your account.");

        // Validation (detailed messages)
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors["title"] = new[] { "Title must not be empty." };
        else if (request.Title.Length > 255)
            errors["title"] = new[] { "Title must be 255 characters or less." };

        if (string.IsNullOrWhiteSpace(request.Street))
            errors["street"] = new[] { "Street is required." };

        if (string.IsNullOrWhiteSpace(request.City))
            errors["city"] = new[] { "City is required." };

        if (string.IsNullOrWhiteSpace(request.State))
            errors["state"] = new[] { "State is required." };

        if (request.Bedrooms < 0)
            errors["bedrooms"] = new[] { "Bedrooms must be non-negative." };

        if (request.Bathrooms < 0)
            errors["bathrooms"] = new[] { "Bathrooms must be non-negative." };

        if (request.Garages < 0)
            errors["garages"] = new[] { "Garages must be non-negative." };

        if (request.Price < 0)
            errors["price"] = new[] { "Price must be non-negative." };

        if (errors.Count > 0)
            throw new ArgumentException("Validation failed."); 

        // Capture "before" snapshot for audit
        var before = new
        {
            entity.Title,
            entity.Description,
            entity.Street,
            entity.City,
            entity.State,
            entity.Postcode,
            entity.PropertyType,
            entity.SaleCategory,
            entity.Bedrooms,
            entity.Bathrooms,
            entity.Garages,
            entity.Price,
            entity.FloorArea,
            entity.Longitude,
            entity.Latitude,
            entity.ListcaseStatus
        };

        // Apply updates
        entity.Title = request.Title.Trim();
        entity.Description = request.Description?.Trim();
        entity.Street = request.Street.Trim();
        entity.City = request.City.Trim();
        entity.State = request.State.Trim();
        entity.Postcode = request.Postcode;
        entity.PropertyType = request.PropertyType;
        entity.SaleCategory = request.SaleCategory;
        entity.Bedrooms = request.Bedrooms;
        entity.Bathrooms = request.Bathrooms;
        entity.Garages = request.Garages;
        entity.Price = request.Price;
        entity.FloorArea = request.FloorArea;
        entity.Longitude = request.Longitude;
        entity.Latitude = request.Latitude;

        await _db.SaveChangesAsync();

        // Capture "after" snapshot
        var after = new
        {
            entity.Title,
            entity.Description,
            entity.Street,
            entity.City,
            entity.State,
            entity.Postcode,
            entity.PropertyType,
            entity.SaleCategory,
            entity.Bedrooms,
            entity.Bathrooms,
            entity.Garages,
            entity.Price,
            entity.FloorArea,
            entity.Longitude,
            entity.Latitude,
            entity.ListcaseStatus
        };

        // Log to MongoDB CaseHistory
        await _history.LogCaseUpdatedAsync(
            entity.Id,
            userId,
            email,
            role,
            ip,
            userAgent,
            changes: new { Before = before, After = after });

        return new ListcaseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Street = entity.Street,
            City = entity.City,
            State = entity.State,
            Postcode = entity.Postcode,
            PropertyType = entity.PropertyType,
            ListcaseStatus = entity.ListcaseStatus,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task DeleteAsync(int id, string userId, string email, string role, string? ip, string? userAgent)
    {
        if (role != "photographyCompany")
            throw new UnauthorizedAccessException("Only photographyCompany can delete a listing.");

        // Load listcase with related to make a snapshot + ensure exists
        var listcase = await _db.Listcases
            .Include(x => x.MediaAssets)
            .Include(x => x.CaseContacts)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (listcase == null)
            throw new ArgumentException($"Listcase {id} not found.");

        // only owner can delete
        if (listcase.UserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to delete this listing.");

        // Optional business rule: Delivered cannot be deleted
        // if (listcase.ListcaseStatus == ListcaseStatus.Delivered)
        //     throw new InvalidOperationException("Delivered listings cannot be deleted.");

        var snapshot = new
        {
            listcase.Id,
            listcase.Title,
            listcase.ListcaseStatus,
            MediaCount = listcase.MediaAssets?.Count ?? 0,
            ContactCount = listcase.CaseContacts?.Count ?? 0
        };

        // IMPORTANT: delete join rows first (AgentListcase)
        var joins = await _db.AgentListcases
            .Where(x => x.ListcaseId == id)
            .ToListAsync();
        if (joins.Count > 0)
            _db.AgentListcases.RemoveRange(joins);

        _db.Listcases.Remove(listcase);

        await _db.SaveChangesAsync();

        await _history.LogCaseDeletedAsync(id, userId, email, role, ip, userAgent, snapshot);
    }

     public async Task<ListcaseDetailDto> GetDetailAsync(int id, string userId, string role)
    {
        // Permission filter built into query for performance + security
        IQueryable<Remp.Models.Entities.Listcase> baseQuery;

        if (role == "photographyCompany")
        {
            baseQuery = _db.Listcases
                .Where(x => x.Id == id && !x.IsDeleted && x.UserId == userId);
        }
        else if (role == "user") // agent
        {
            baseQuery = _db.AgentListcases
                .Where(al => al.AgentId == userId && al.ListcaseId == id)
                .Select(al => al.Listcase)
                .Where(x => !x.IsDeleted);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid role.");
        }

        // One SQL roundtrip: project only fields we need
        var dto = await baseQuery
            .AsNoTracking()
            .Select(x => new ListcaseDetailDto
            {
                Id = x.Id,
                Title = x.Title,

                Street = x.Street,
                City = x.City,
                State = x.State,
                Postcode = x.Postcode,

                Longitude = x.Longitude,
                Latitude = x.Latitude,

                Price = x.Price,
                Bedrooms = x.Bedrooms,
                Bathrooms = x.Bathrooms,
                Garages = x.Garages,
                FloorArea = x.FloorArea,

                PropertyType = x.PropertyType,
                SaleCategory = x.SaleCategory,
                ListcaseStatus = x.ListcaseStatus,

                CreatedAt = x.CreatedAt,

                MediaAssets = x.MediaAssets
                    .Where(m => !m.IsDeleted)
                    .OrderByDescending(m => m.UploadedAt)
                    .Select(m => new MediaAssetDto
                    {
                        Id = m.Id,
                        MediaType = m.MediaType,
                        MediaUrl = m.MediaUrl,
                        UploadedAt = m.UploadedAt,
                        IsSelect = m.IsSelect,
                        IsHero = m.IsHero
                    })
                    .ToList(),

                Agents = x.AgentListcases
                    .Select(al => al.Agent)
                    .Select(a => new AgentDto
                    {
                        Id = a.Id,
                        AgentFirstName = a.AgentFirstName,
                        AgentLastName = a.AgentLastName,
                        AvatarUrl = a.AvatarUrl,
                        CompanyName = a.CompanyName
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (dto == null)
            throw new ArgumentException($"Listing {id} not found or you have no access.");

        return dto;
    }

    public async Task ChangeStatusAsync(
        int id,
        ListcaseStatus newStatus,
        string userId,
        string email,
        string role,
        string? ip,
        string? userAgent)
    {
        // Only photographyCompany can change status
        if (role != "photographyCompany")
            throw new UnauthorizedAccessException("Only photographyCompany can change listing status.");

        var entity = await _db.Listcases.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
            throw new ArgumentException($"Listcase {id} not found.");

        // Only owner can change status
        if (entity.UserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to change this listing status.");

        var current = entity.ListcaseStatus;

        if (current == newStatus)
            throw new InvalidOperationException("New status is the same as current status.");

        // Enforce workflow: Created -> Pending -> Delivered
        if (!IsValidTransition(current, newStatus))
            throw new InvalidOperationException($"Invalid status transition: {current} -> {newStatus}.");

        entity.ListcaseStatus = newStatus;
        await _db.SaveChangesAsync();

        await _history.LogStatusChangedAsync(id, current, newStatus, userId, email, role, ip, userAgent);
    }

    private static bool IsValidTransition(ListcaseStatus from, ListcaseStatus to)
    {
        return (from == ListcaseStatus.Created && to == ListcaseStatus.Pending)
            || (from == ListcaseStatus.Pending && to == ListcaseStatus.Delivered);
    }

    public async Task<List<ListMediaGroupDto>> GetMediaGroupedAsync(int listcaseId, string userId, string role)
    {
        // Permission check by role:
        // photographyCompany: must own the listcase
        // user(agent): must be assigned via AgentListcases

        bool hasAccess;

        if (role == "photographyCompany")
        {
            hasAccess = await _db.Listcases
                .AsNoTracking()
                .AnyAsync(x => x.Id == listcaseId && !x.IsDeleted && x.UserId == userId);
        }
        else if (role == "user")
        {
            hasAccess = await _db.AgentListcases
                .AsNoTracking()
                .AnyAsync(x => x.ListcaseId == listcaseId && x.AgentId == userId);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid role.");
        }

        if (!hasAccess)
            throw new UnauthorizedAccessException("You do not have access to this listing.");

        // Fetch media list (SQL projection)
        var media = await _db.MediaAssets
            .AsNoTracking()
            .Where(m => m.ListcaseId == listcaseId && !m.IsDeleted)
            .Select(m => new ListingMediaItemDto
            {
                Id = m.Id,
                MediaUrl = m.MediaUrl,
                UploadedAt = m.UploadedAt,
                IsSelect = m.IsSelect,
                IsHero = m.IsHero
            })
            .ToListAsync();

        // Group by MediaType (need MediaType in projection)
        // If your MediaAsset has MediaType property (it should), use it directly:
        var grouped = await _db.MediaAssets
            .AsNoTracking()
            .Where(m => m.ListcaseId == listcaseId && !m.IsDeleted)
            .Select(m => new
            {
                m.MediaType,
                Item = new ListingMediaItemDto
                {
                    Id = m.Id,
                    MediaUrl = m.MediaUrl,
                    UploadedAt = m.UploadedAt,
                    IsSelect = m.IsSelect,
                    IsHero = m.IsHero
                }
            })
            .ToListAsync();

        return grouped
            .GroupBy(x => x.MediaType)
            .Select(g => new ListMediaGroupDto
            {
                MediaType = g.Key,
                Items = g.Select(x => x.Item)
                        .OrderByDescending(x => x.UploadedAt)
                        .ToList()
            })
            .OrderBy(g => g.MediaType) // optional
            .ToList();
    }
}