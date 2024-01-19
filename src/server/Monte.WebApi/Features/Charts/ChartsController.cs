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

    [HttpGet("{agentId:guid}/cpu/usage")]
    public Task<ChartData<double>> GetCpuUsage(
        Guid agentId,
        [FromQuery] GetCpuUsageChartDataRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToQuery(agentId), cancellationToken);
    
    [HttpGet("{agentId:guid}/cpu/load")]
    public Task<ChartData<double>> GetCpuUsage(
        Guid agentId,
        [FromQuery] GetCpuLoadChartDataRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToQuery(agentId), cancellationToken);
    
    [HttpGet("{agentId:guid}/memory/usage")]
    public Task<ChartData<double>> GetMemoryUsage(
        Guid agentId,
        [FromQuery] GetMemoryChartDataRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToUsageChartQuery(agentId), cancellationToken);
    
    [HttpGet("{agentId:guid}/memory/available")]
    public Task<ChartData<double>> GetAvailableMemory(
        Guid agentId,
        [FromQuery] GetMemoryChartDataRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToAvailableChartQuery(agentId), cancellationToken);
}
