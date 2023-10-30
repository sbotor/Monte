using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.Features.Machines.Commands;
using Monte.Features.Machines.Queries;
using Monte.WebApi.Auth;

namespace Monte.WebApi.Features.Machines;

[ApiController]
[Route("api/[controller]")]
public class MachinesController : ControllerBase
{
    private readonly ISender _sender;

    public MachinesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(AuthSetup.RequireMainApiScope)]
    public async Task<IActionResult> GetMachines(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetMachines.Query(), cancellationToken));

    [HttpPut]
    [Authorize(AuthSetup.RequireAgentApiScope)]
    public async Task<IActionResult> UpsertMachine()
        => Ok(await _sender.Send(new UpsertMachine.Command()));
}
