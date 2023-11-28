namespace Monte.Features.Machines.Models;

public class MachineOverview
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public DateTime LastHeartbeat { get; set; }
    public DateTime Created { get; set; }
}
