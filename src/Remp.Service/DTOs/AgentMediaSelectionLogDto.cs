namespace Remp.Service.DTOs
{
    public class AgentMediaSelectionLogDto
    {
        public string AgentId { get; set; } = string.Empty;
        public int ListingId { get; set; }
        public List<int> MediaIds { get; set; } = new();
        public DateTime TimestampUtc { get; set; }
    }
}