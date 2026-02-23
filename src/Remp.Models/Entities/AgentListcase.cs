namespace Remp.Models.Entities;

public class AgentListcase
{
    public string AgentId { get; set; } = default!;

    public int ListcaseId { get; set; }

    public Agent Agent { get; set; } = default!;

    public Listcase Listcase { get; set; } = default!;
}