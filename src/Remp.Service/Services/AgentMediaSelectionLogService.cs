using MongoDB.Driver;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services
{
    public class AgentMediaSelectionLogService : IAgentMediaSelectionLogService
    {
        private readonly IMongoCollection<AgentMediaSelectionLogDto> _collection;

        public AgentMediaSelectionLogService(IMongoDatabase database)
        {
            _collection = database.GetCollection<AgentMediaSelectionLogDto>("AgentMediaSelectionLogs");
        }

        public async Task LogSelectionAsync(string agentId, int listingId, List<int> mediaIds, DateTime timestampUtc)
        {
            var log = new AgentMediaSelectionLogDto
            {
                AgentId = agentId,
                ListingId = listingId,
                MediaIds = mediaIds,
                TimestampUtc = timestampUtc
            };

            await _collection.InsertOneAsync(log);
        }
    }
}