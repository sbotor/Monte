using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.WebApi.Auth;
using Monte.WebApi.Features.Charts.Requests;

namespace Monte.WebApi.Features.Charts;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
public class ChartsController : ControllerBase
{
    private readonly ISender _sender;

    public ChartsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{machineId:guid}/cpu")]
    public async Task<IActionResult> GetCpuUsage(
        Guid machineId,
        [FromQuery] GetCpuUsageChartDataRequest request,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(request.ToQuery(machineId), cancellationToken));
    
    [HttpGet("{machineId:guid}/memory")]
    public async Task<IActionResult> GetMemoryUsage(
        Guid machineId,
        [FromQuery] GetMemoryUsageChartDataRequest request,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(request.ToQuery(machineId), cancellationToken));
}
