namespace Remp.Service.Interfaces
{
    public interface IAgentMediaSelectionLogService
    {
        Task LogSelectionAsync(string agentId, int listingId, List<int> mediaIds, DateTime timestampUtc);
    }
}