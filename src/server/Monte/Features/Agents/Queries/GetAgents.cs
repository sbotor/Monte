using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Extensions;
using Monte.Features.Agents.Models;
using Monte.Models;

namespace Monte.Features.Agents.Queries;

public static class GetAgents
{
    public record Query(Paging Paging, Sorting Sorting) : IRequest<PagedResponse<AgentOverview>>;

    internal class Handler : IRequestHandler<Query, PagedResponse<AgentOverview>>
    {
        private readonly MonteDbContext _context;

        public Handler(MonteDbContext context)
        {
            _context = context;
        }

        public Task<PagedResponse<AgentOverview>> Handle(Query request, CancellationToken cancellationToken)
            => Order(_context.Agents.AsNoTracking(), request.Sorting)
                .Select(x => new AgentOverview
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    LastHeartbeat = x.HeartbeatDateTime,
                    Created = x.CreatedDateTime
                })
                .PaginateAsync(request.Paging, cancellationToken);

        private static IOrderedQueryable<Agent> Order(IQueryable<Agent> query, Sorting sorting)
            => sorting.Value switch
            {
                "lastHeartbeat" => query.OrderBy(x => x.HeartbeatDateTime),
                "created" => query.OrderBy(x => x.CreatedDateTime),
                _ => query.OrderBy(x => x.Name).ThenBy(x => x.OrdinalNumber)
            };
    }
}
