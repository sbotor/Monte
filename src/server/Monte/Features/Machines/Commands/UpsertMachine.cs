using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Models.Exceptions;
using Monte.Services;

namespace Monte.Features.Machines.Commands;

public static class UpsertMachine
{
    public record Command(Machine.CpuInfo Cpu, Machine.MemoryInfo Memory) : IRequest<string>;

    internal class Handler : IRequestHandler<Command, string>
    {
        private readonly MonteDbContext _context;
        private readonly IAgentContextProvider _agentContextProvider;
        private readonly IClock _clock;

        public Handler(
            MonteDbContext context,
            IAgentContextProvider agentContextProvider,
            IClock clock)
        {
            _context = context;
            _agentContextProvider = agentContextProvider;
            _clock = clock;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var agentContext = await _agentContextProvider.GetContext(CancellationToken.None);

            var agentId = !agentContext.Id.HasValue
                ? await CreateNew(agentContext.Origin, request)
                : await UpdateMachine(agentContext.Id.Value, request);

            return agentId;
        }

        private async Task<string> CreateNew(string origin, Command command)
        {
            var lastExistingMachine = await _context.Machines.AsNoTracking()
                .Where(x => x.Name == origin)
                .OrderByDescending(x => x.OrdinalNumber)
                .FirstOrDefaultAsync();

            var machineNumber = lastExistingMachine?.OrdinalNumber + 1 ?? 0;

            var now = _clock.UtcNow;
            var machine = new Machine
            {
                CreatedDateTime = now,
                HeartbeatDateTime = now,
                OrdinalNumber = machineNumber,
                Name = origin,
                Cpu = new(),
                Memory = new()
            };
            machine.Cpu.Update(command.Cpu);
            machine.Memory.Update(command.Memory);
            
            _context.Add(machine);
            await _context.SaveChangesAsync();

            return machine.Id.ToString();
        }

        private async Task<string> UpdateMachine(Guid id, Command command)
        {
            var machine = await _context.Machines
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException();

            machine.HeartbeatDateTime = _clock.UtcNow;
            machine.Cpu.Update(command.Cpu);
            machine.Memory.Update(command.Memory);
            
            await _context.SaveChangesAsync();

            return machine.Id.ToString();
        }
    }
}
