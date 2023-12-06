using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monte.WebApi.Features.Charts.Requests;

namespace Monte.WebApi.Features.Charts;

[ApiController]
[Route("api/[controller]")]
public class ChartsController : ControllerBase
{
    private readonly ISender _sender;

    public ChartsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{machineId:guid}/cpu/avg")]
    public async Task<IActionResult> GetAvgCpuUsage(
        Guid machineId,
        [FromQuery] GetAvgCpuUsageChartDataRequest request,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(request.ToQuery(machineId), cancellationToken));
}
