using Monte.Features.Agents.Queries;
using Monte.Models;

namespace Monte.WebApi.Features.Agents.Requests;

public class GetAgentsRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; } = Paging.DefaultPageSize;
    public string? OrderBy { get; set; }
    public bool OrderByDesc { get; set; }

    public GetAgents.Query ToQuery()
        => new(new(Page, PageSize), new(OrderBy, OrderByDesc));
}
