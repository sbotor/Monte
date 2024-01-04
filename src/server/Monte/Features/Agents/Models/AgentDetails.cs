namespace Monte.Features.Agents.Models;

public class AgentDetails
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public int CpuLogicalCount { get; set; }
}
