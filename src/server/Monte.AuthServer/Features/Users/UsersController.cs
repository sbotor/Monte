using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;
using Monte.AuthServer.Services;
using OpenIddict.Abstractions;

namespace Monte.AuthServer.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("root")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRootAdmin(CreateUserRequest request)
    {
        var result = await _userService.CreateUser(request, AuthConsts.Roles.MonteAdmin);
        if (result.ErrType == Result.ErrorType.None)
        {
            return Created(Request.Path, result.Object);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }

    [HttpPost]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userService.CreateUser(request, AuthConsts.Roles.MonteUser);
        if (result.ErrType == Result.ErrorType.None)
        {
            return Created(Request.Path, result.Object);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }

    [HttpGet]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userService.GetUsers();
        if (result.ErrType == Result.ErrorType.None)
        {
            return Ok(result.Object);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var result = await _userService.DeleteUser(userId);
        if (result.ErrType == Result.ErrorType.None)
        {
            return NoContent();
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }


    [HttpPost("password")]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var isAdmin = HttpContext.User.IsInRole(AuthConsts.Roles.MonteAdmin);

        var RequesterId = HttpContext.User.GetClaim("sub");

        if(RequesterId.IsNullOrEmpty() && (request.UserId.IsNullOrEmpty() || !isAdmin))
        {
            return BadRequest("The request does not contain information about the user");
        }
        if(!isAdmin || (isAdmin && request.UserId.IsNullOrEmpty()))
        {
            request.UserId = RequesterId;
        }

        var result = await _userService.ChangePassword(request);

        if (result.ErrType == Result.ErrorType.None)
        {
            return NoContent();
        }
        else if (result.ErrType == Result.ErrorType.NotFound)
        {
            return NotFound(result.ErrorMessage);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }

    [HttpPost("username")]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUsername(ChangeUsernameRequest request)
    {
        var isAdmin = HttpContext.User.IsInRole(AuthConsts.Roles.MonteAdmin);
        var RequesterId = HttpContext.User.GetClaim("sub");

        if (RequesterId.IsNullOrEmpty() && (request.UserId.IsNullOrEmpty() || !isAdmin))
        {
            return BadRequest("The request does not contain information about the user");
        }
        if (!isAdmin || (isAdmin && request.UserId.IsNullOrEmpty()))
        {
            request.UserId = RequesterId;
        }

        var result = await _userService.ChangeUsername(request);

        if (result.ErrType == Result.ErrorType.None)
        {
            return Ok(result.Object);
        }
        else if (result.ErrType == Result.ErrorType.NotFound)
        {
            return NotFound(result.ErrorMessage);
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }
    }
}
