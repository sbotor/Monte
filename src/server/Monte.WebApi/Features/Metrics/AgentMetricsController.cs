using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.Features.Agents.Models;
using Monte.WebApi.Auth;
using Monte.WebApi.Features.Metrics.Requests;

namespace Monte.WebApi.Features.Metrics;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AuthConsts.Roles.MonteAgent)]
public class AgentMetricsController : ControllerBase
{
    private readonly ISender _sender;

    public AgentMetricsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPut("init")]
    public Task<AgentInitializationDetails> InitializeAgent(InitializeAgentRequest request)
        => _sender.Send(request.ToCommand());

    [HttpPost]
    public Task<string> Report(ReportMetricsRequest request)
        => _sender.Send(request.ToCommand());
}
