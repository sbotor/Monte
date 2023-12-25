using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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


    [HttpPatch("password")]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(string? userId, string oldPassword, string newPassword)
    {
        var isAdmin = HttpContext.User.IsInRole(AuthConsts.Roles.MonteAdmin);

        var RequesterId = HttpContext.User.GetClaim("sub");

        Result? result;
        if(isAdmin && !userId.IsNullOrEmpty())
        {
            result = await _userService.ChangePassword(userId!, oldPassword, newPassword);
        }
        else
        {
            if (RequesterId != null)
                result = await _userService.ChangePassword(RequesterId, oldPassword, newPassword);
            else
                return BadRequest("The request does not contain information about the user");
        }

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

    [HttpPatch("username")]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUsername(string? userId, string newUsername)
    {
        var isAdmin = HttpContext.User.IsInRole(AuthConsts.Roles.MonteAdmin);
        var RequesterId = HttpContext.User.GetClaim("sub");

        Result<UserDetails>? result;
        if (isAdmin && !userId.IsNullOrEmpty())
        {
            result = await _userService.ChangeUsername(userId!, newUsername);
        }
        else
        {
            if (RequesterId != null)
                result = await _userService.ChangeUsername(RequesterId, newUsername);
            else
                return BadRequest("The request does not contain information about the user");
        }

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
