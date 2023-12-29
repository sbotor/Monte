using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.Features.Machines.Queries;
using Monte.WebApi.Auth;
using Monte.WebApi.Features.Machines.Requests;

namespace Monte.WebApi.Features.Machines;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
public class MachinesController : ControllerBase
{
    private readonly ISender _sender;

    public MachinesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetMachines([FromQuery] GetMachinesRequest request,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(request.ToQuery(), cancellationToken));
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMachines(Guid id,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetMachineDetails.Query(id), cancellationToken));
}
