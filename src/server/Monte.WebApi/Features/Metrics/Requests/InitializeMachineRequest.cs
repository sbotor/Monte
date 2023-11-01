using Monte.Features.Machines;
using Monte.Features.Machines.Commands;

namespace Monte.WebApi.Features.Metrics.Requests;

public class InitializeMachineRequest
{
    public Machine.CpuInfo Cpu { get; set; } = null!;
    public Machine.MemoryInfo Memory { get; set; } = null!;

    public UpsertMachine.Command ToCommand()
        => new(Cpu, Memory);
}
