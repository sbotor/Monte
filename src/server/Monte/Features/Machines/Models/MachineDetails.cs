namespace Monte.Features.Machines.Models;

public class MachineDetails
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public int CpuLogicalCount { get; set; }
}
