using Remp.Common.Utilities;
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
        // Thorough validation (minimum required set)
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

        // Create entity, SQL Server will generate unique int ID
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
            ListcaseStatus = ListcaseStatus.Created, // initial status = Created
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId
        };

        _db.ListingCases.Add(entity);
        await _db.SaveChangesAsync();

        // Audit to MongoDB CaseHistory
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
}