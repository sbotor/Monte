using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Features.Agents.Models;
using Monte.Models.Exceptions;
using Monte.Services;

namespace Monte.Features.Agents.Commands;

public static class UpsertAgent
{
    public record Command(Agent.CpuInfo Cpu, Agent.MemoryInfo Memory) : IRequest<AgentInitializationDetails>;

    internal class Handler : IRequestHandler<Command, AgentInitializationDetails>
    {
        private readonly MonteDbContext _context;
        private readonly IAgentContextProvider _agentContextProvider;
        private readonly IClock _clock;
        private readonly IMetricsKeyGenerator _metricsKeyGenerator;

        public Handler(
            MonteDbContext context,
            IAgentContextProvider agentContextProvider,
            IClock clock,
            IMetricsKeyGenerator metricsKeyGenerator)
        {
            _context = context;
            _agentContextProvider = agentContextProvider;
            _clock = clock;
            _metricsKeyGenerator = metricsKeyGenerator;
        }

        public async Task<AgentInitializationDetails> Handle(Command request, CancellationToken cancellationToken)
        {
            var agentContext = await _agentContextProvider.GetContext(CancellationToken.None);

            var agent = !agentContext.Id.HasValue
                ? await CreateNewAgent(agentContext.Origin, request)
                : await UpdateAgent(agentContext.Id.Value, request);

            return new(agent.Id.ToString(), agent.MetricsKey);
        }

        private async Task<Agent> CreateNewAgent(string origin, Command command)
        {
            var lastExistingAgent = await _context.Agents.AsNoTracking()
                .Where(x => x.Name == origin)
                .OrderByDescending(x => x.OrdinalNumber)
                .FirstOrDefaultAsync();

            var agentNumber = lastExistingAgent?.OrdinalNumber + 1 ?? 0;

            var now = _clock.UtcNow;
            var agent = new Agent
            {
                CreatedDateTime = now,
                HeartbeatDateTime = now,
                OrdinalNumber = agentNumber,
                Name = origin,
                Cpu = new(),
                Memory = new()
            };
            
            agent.Cpu.Update(command.Cpu);
            agent.Memory.Update(command.Memory);
            agent.MetricsKey = _metricsKeyGenerator.GenerateKey();
            
            _context.Add(agent);
            await _context.SaveChangesAsync();

            return agent;
        }

        private async Task<Agent> UpdateAgent(Guid id, Command command)
        {
            var agent = await _context.Agents
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException();

            agent.HeartbeatDateTime = _clock.UtcNow;
            agent.Cpu.Update(command.Cpu);
            agent.Memory.Update(command.Memory);
            agent.MetricsKey = _metricsKeyGenerator.GenerateKey();
            
            await _context.SaveChangesAsync();

            return agent;
        }
    }
}
