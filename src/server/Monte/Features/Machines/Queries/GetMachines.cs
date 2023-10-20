using Microsoft.EntityFrameworkCore;
using Monte.Cqrs;
using Monte.Features.Machines.Models;

namespace Monte.Features.Machines.Queries;

public static class GetMachines
{
    public record Query : IQuery<MachineOverview[]>;

    internal class Handler : IQueryHandler<Query, MachineOverview[]>
    {
        private readonly MonteDbContext _context;

        public Handler(MonteDbContext context)
        {
            _context = context;
        }

        public Task<MachineOverview[]> Handle(Query request, CancellationToken cancellationToken)
            => _context.Machines.AsNoTracking()
                .OrderBy(x => x.DisplayName)
                .Select(x => new MachineOverview { Id = x.Id, DisplayName = x.DisplayName })
                .ToArrayAsync(cancellationToken);
    }
}