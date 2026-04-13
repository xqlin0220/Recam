using MongoDB.Driver;
using RealEstate.Collection;

namespace RealEstate.Repository.StatusHistoryRepository
{
    public class StatusHistoryRepository : IStatusHistoryRepository
    {
        private readonly MongoDbContext _mongoDbContext;

        public StatusHistoryRepository(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<List<StatusHistory>> GetByListingCaseIdsAsync(List<string> listingCaseIds)
        {
            var filter = Builders<StatusHistory>.Filter.In(x => x.ListingCaseId, listingCaseIds);
            return await _mongoDbContext.StatusHistories.Find(filter).ToListAsync();
        }
    }
}
