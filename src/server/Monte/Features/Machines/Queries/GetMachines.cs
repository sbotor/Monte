using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Extensions;
using Monte.Features.Machines.Models;
using Monte.Models;

namespace Monte.Features.Machines.Queries;

public static class GetMachines
{
    public record Query(Paging Paging, Sorting Sorting) : IRequest<PagedResponse<MachineOverview>>;

    internal class Handler : IRequestHandler<Query, PagedResponse<MachineOverview>>
    {
        private readonly MonteDbContext _context;

        public Handler(MonteDbContext context)
        {
            _context = context;
        }

        public Task<PagedResponse<MachineOverview>> Handle(Query request, CancellationToken cancellationToken)
            => Order(_context.Machines.AsNoTracking(), request.Sorting)
                .Select(x => new MachineOverview
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    LastHeartbeat = x.HeartbeatDateTime,
                    Created = x.CreatedDateTime
                })
                .PaginateAsync(request.Paging, cancellationToken);

        private static IOrderedQueryable<Machine> Order(IQueryable<Machine> query, Sorting sorting)
            => sorting.Value switch
            {
                "lastHeartbeat" => query.OrderBy(x => x.HeartbeatDateTime),
                "created" => query.OrderBy(x => x.CreatedDateTime),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.OrdinalNumber)
            };
    }
}
