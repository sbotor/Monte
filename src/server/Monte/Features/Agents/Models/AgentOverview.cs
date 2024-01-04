namespace Monte.Features.Agents.Models;

public class AgentOverview
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public DateTime LastHeartbeat { get; set; }
    public DateTime Created { get; set; }
}
