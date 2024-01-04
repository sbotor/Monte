using Monte.Features.Agents;
using Monte.Features.Agents.Commands;

namespace Monte.WebApi.Features.Metrics.Requests;

public class InitializeAgentRequest
{
    public Agent.CpuInfo Cpu { get; set; } = null!;
    public Agent.MemoryInfo Memory { get; set; } = null!;

    public UpsertAgent.Command ToCommand()
        => new(Cpu, Memory);
}
