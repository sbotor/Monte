using Microsoft.EntityFrameworkCore;
using Monte.Cqrs;
using Monte.Exceptions;

namespace Monte.Features.Metrics.Commands;

public static class ReportMetrics
{
    public record Command(Command.CpuUsage Cpu, Command.MemoryUsage Memory) : ICommand
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

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MonteDbContext _dbContext;
        private readonly IAgentContextProvider _agentContextProvider;
        private readonly IClock _clock;

        public Handler(MonteDbContext dbContext, IAgentContextProvider agentContextProvider, IClock clock)
        {
            _dbContext = dbContext;
            _agentContextProvider = agentContextProvider;
            _clock = clock;
        }
        
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var agentContext = await _agentContextProvider.GetContext(CancellationToken.None);
            var agentId = agentContext.Id ?? throw new BadRequestException();
            var now = _clock.UtcNow;
            
            var machine = await _dbContext.Machines.FirstOrDefaultAsync(x => x.Id == agentId, CancellationToken.None)
                ?? throw new NotFoundException();
            machine.HeartbeatDateTime = now;

            _dbContext.Add(new MetricsEntry
            {
                MachineId = agentId,
                ReportDateTime = now,
            });

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
