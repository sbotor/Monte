using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.Features.Charts.Models;
using Monte.WebApi.Auth;
using Monte.WebApi.Features.Charts.Requests;

namespace Monte.WebApi.Features.Charts;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AuthConsts.Groups.AllUsers)]
public class ChartsController : ControllerBase
{
    private readonly ISender _sender;

    public ChartsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{agentId:guid}/cpu")]
    public Task<ChartData<double>> GetCpuUsage(
        Guid agentId,
        [FromQuery] GetCpuUsageChartDataRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToQuery(agentId), cancellationToken);
    
    [HttpGet("{agentId:guid}/memory")]
    public Task<ChartData<double>> GetMemoryUsage(
        Guid agentId,
        [FromQuery] GetMemoryUsageChartDataRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToQuery(agentId), cancellationToken);
}
