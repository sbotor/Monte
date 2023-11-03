using static Monte.Features.Metrics.Commands.ReportMetrics;

namespace Monte.WebApi.Features.Metrics.Requests;

public class ReportMetricsRequest
{
    public Command.CpuUsage? Cpu { get; set; }
    public Command.MemoryUsage? Memory { get; set; }

    public Command ToCommand()
        => new(Cpu ?? new(), Memory ?? new());
}
