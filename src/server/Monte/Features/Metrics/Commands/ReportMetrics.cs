using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Contracts;
using Monte.Models.Exceptions;
using Monte.Services;

namespace Monte.Features.Metrics.Commands;

public static class ReportMetrics
{
    public record Command(Command.CpuUsage Cpu, Command.MemoryUsage Memory, string MetricsKey)
        : IRequest<string>, IMetricsKeyRequest
    {
        public class CpuUsage
        {
            public IReadOnlyCollection<double> PercentsUsed { get; set; } = Array.Empty<double>();
            public double Load { get; set; }
        }

        public class MemoryUsage
        {
            public ulong Available { get; set; }
            public double PercentUsed { get; set; }
            public ulong SwapAvailable { get; set; }
            public double SwapPercentUsed { get; set; }
        }
    }

    internal class Handler : IRequestHandler<Command, string>
    {
        private readonly MonteDbContext _dbContext;
        private readonly IAgentContextProvider _agentContextProvider;
        private readonly IClock _clock;
        private readonly IMetricsKeyGenerator _metricsKeyGenerator;

        public Handler(
            MonteDbContext dbContext,
            IAgentContextProvider agentContextProvider,
            IClock clock,
            IMetricsKeyGenerator metricsKeyGenerator)
        {
            _dbContext = dbContext;
            _agentContextProvider = agentContextProvider;
            _clock = clock;
            _metricsKeyGenerator = metricsKeyGenerator;
        }
        
        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var agentContext = await _agentContextProvider.GetContext(CancellationToken.None);
            var agentId = agentContext.RequiredId;
            var now = _clock.UtcNow;
            
            var agent = await _dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentId, CancellationToken.None)
                ?? throw new NotFoundException();
            agent.HeartbeatDateTime = now;
            agent.MetricsKey = _metricsKeyGenerator.GenerateKey();

            var cpu = request.Cpu;
            var memory = request.Memory;
            
            _dbContext.Add(new MetricsEntry
            {
                AgentId = agentId,
                ReportDateTime = now,
                Cores = cpu.PercentsUsed.Select((x, i) => new CoreUsageEntry
                {
                    Ordinal = i,
                    PercentUsed = x
                }).ToList(),
                Cpu = new()
                {
                    AveragePercentUsed = cpu.PercentsUsed.Average(),
                    Load = cpu.Load
                },
                Memory = new()
                {
                    Available = memory.Available,
                    PercentUsed = memory.PercentUsed,
                    SwapAvailable = memory.SwapAvailable,
                    SwapPercentUsed = memory.SwapPercentUsed
                }
            });

            await _dbContext.SaveChangesAsync(CancellationToken.None);

            return agent.MetricsKey;
        }
    }
}
