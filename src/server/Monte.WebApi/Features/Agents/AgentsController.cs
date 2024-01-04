using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.Features.Agents.Models;
using Monte.Features.Agents.Queries;
using Monte.Models;
using Monte.WebApi.Auth;
using Monte.WebApi.Features.Agents.Requests;

namespace Monte.WebApi.Features.Agents;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AuthConsts.Groups.AllUsers)]
public class AgentsController : ControllerBase
{
    private readonly ISender _sender;

    public AgentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public Task<PagedResponse<AgentOverview>> GetAgents([FromQuery] GetAgentsRequest request,
        CancellationToken cancellationToken)
        => _sender.Send(request.ToQuery(), cancellationToken);
    
    [HttpGet("{id:guid}")]
    public Task<AgentDetails> GetAgent(Guid id,
        CancellationToken cancellationToken)
        => _sender.Send(new GetAgentDetails.Query(id), cancellationToken);
}
