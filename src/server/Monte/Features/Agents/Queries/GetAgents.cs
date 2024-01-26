using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Extensions;
using Monte.Features.Agents.Models;
using Monte.Models;

namespace Monte.Features.Agents.Queries;

public static class GetAgents
{
    public record Query(
        Paging Paging,
        Sorting<AgentSorting> Sorting,
        string? TextFilter)
        : IRequest<PagedResponse<AgentOverview>>;

    internal class Handler : IRequestHandler<Query, PagedResponse<AgentOverview>>
    {
        private readonly MonteDbContext _context;

        public Handler(MonteDbContext context)
        {
            _context = context;
        }

        public Task<PagedResponse<AgentOverview>> Handle(Query request, CancellationToken cancellationToken)
            => Order(_context.Agents.AsNoTracking(), request.Sorting)
                .Where(x => x.Name.Contains(request.TextFilter!),
                    !string.IsNullOrEmpty(request.TextFilter))
                .Select(x => new AgentOverview
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    LastHeartbeat = x.HeartbeatDateTime,
                    Created = x.CreatedDateTime
                })
                .PaginateAsync(request.Paging, cancellationToken);

        private static IOrderedQueryable<Agent> Order(IQueryable<Agent> query, Sorting<AgentSorting> sorting)
            => sorting.Value switch
            {
                AgentSorting.LastHeartbeat => query.OrderBy(x => x.HeartbeatDateTime, sorting.Descending),
                AgentSorting.Created => query.OrderBy(x => x.CreatedDateTime, sorting.Descending),
                AgentSorting.Name => query.OrderBy(x => x.Name, sorting.Descending)
                    .ThenBy(x => x.OrdinalNumber, sorting.Descending),
                _ => query.OrderBy(x => x.Name, sorting.Descending)
                    .ThenBy(x => x.OrdinalNumber, sorting.Descending)
            };
    }
}
