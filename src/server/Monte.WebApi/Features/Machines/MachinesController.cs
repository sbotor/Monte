using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monte.Features.Machines.Queries;
using Monte.WebApi.Auth;

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
    public async Task<IActionResult> GetMachines(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetMachines.Query(), cancellationToken));
}
