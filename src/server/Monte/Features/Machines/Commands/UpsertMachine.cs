using Monte.Cqrs;

namespace Monte.Features.Machines.Commands;

public static class UpsertMachine
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MonteDbContext _context;
        private readonly IAgentContextProvider _agentContextProvider;

        public Handler(MonteDbContext context, IAgentContextProvider agentContextProvider)
        {
            _context = context;
            _agentContextProvider = agentContextProvider;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var agentContext = await _agentContextProvider.GetContext(CancellationToken.None);

            throw new NotImplementedException();
        }
    }
}
