using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Features.Agents.Models;
using Monte.Models.Exceptions;

namespace Monte.Features.Agents.Queries;

public static class GetAgentDetails
{
    public record Query(Guid Id) : IRequest<AgentDetails>;

    internal class Handler : IRequestHandler<Query, AgentDetails>
    {
        private readonly MonteDbContext _dbContext;

        public Handler(MonteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AgentDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var agent = await _dbContext.Agents.AsNoTracking()
                .Include(x => x.Cpu)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException();

            return new(agent.Id,
                agent.HeartbeatDateTime,
                agent.DisplayName,
                agent.Cpu,
                agent.Memory);
        }
    }
}
