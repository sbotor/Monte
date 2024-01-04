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
            var machine = await _dbContext.Agents.AsNoTracking()
                .Include(x => x.Cpu)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException();

            return new()
            {
                Id = machine.Id,
                DisplayName = machine.DisplayName,
                CpuLogicalCount = machine.Cpu.LogicalCount
            };
        }
    }
}
