using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.WebApi.Auth;
using Monte.WebApi.Features.Metrics.Requests;

namespace Monte.WebApi.Features.Metrics;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthSetup.RequireAgentApiScope)]
public class AgentMetricsController : ControllerBase
{
    private readonly ISender _sender;

    public AgentMetricsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpPut("init")]
    public async Task<IActionResult> InitializeMachine(InitializeMachineRequest request)
        => Ok(await _sender.Send(request.ToCommand()));

    [HttpPost]
    public async Task<IActionResult> Report(ReportMetricsRequest request)
    {
        await _sender.Send(request.ToCommand());
        return Ok();
    }
}
