using Monte.Features.Machines.Queries;
using Monte.Models;

namespace Monte.WebApi.Features.Machines.Requests;

public class GetMachinesRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; } = Paging.DefaultPageSize;
    public string? OrderBy { get; set; }
    public bool OrderByDesc { get; set; }

    public GetMachines.Query ToQuery()
        => new(new(Page, PageSize), new(OrderBy, OrderByDesc));
}
