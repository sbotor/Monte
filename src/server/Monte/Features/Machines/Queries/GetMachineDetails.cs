using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Features.Machines.Models;
using Monte.Models.Exceptions;

namespace Monte.Features.Machines.Queries;

public static class GetMachineDetails
{
    public record Query(Guid Id) : IRequest<MachineDetails>;

    internal class Handler : IRequestHandler<Query, MachineDetails>
    {
        private readonly MonteDbContext _dbContext;

        public Handler(MonteDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<MachineDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var machine = await _dbContext.Machines.AsNoTracking()
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
