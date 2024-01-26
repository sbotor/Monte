using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Monte.AuthServer.Extensions;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;
using Monte.AuthServer.Models;
using Monte.AuthServer.Services;
using OpenIddict.Abstractions;

namespace Monte.AuthServer.Features.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = AuthSchemes.TokenValidation)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userService.CreateUser(request, AuthConsts.Roles.MonteUser);
        return result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userService.GetUsers();
        return result.ToActionResult();
    }
    
    [HttpGet("{userId}")]
    [Authorize(Roles = AuthConsts.RoleGroups.AllUsers)]
    public async Task<IActionResult> GetUser(string userId)
    {
        var result = await _userService.GetUser(userId);
        return result.ToActionResult();
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var result = await _userService.DeleteUser(userId);
        return result.ToActionResult();
    }


    [HttpPost("{userId}/password")]
    [Authorize(Roles = AuthConsts.RoleGroups.AllUsers)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        string userId,
        ChangePasswordRequest request)
    {
        var result = await _userService.ChangePassword(userId, request);
        return result.ToActionResult();
    }

    [HttpPost("{userId}/username")]
    [Authorize(Roles = AuthConsts.RoleGroups.AllUsers)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUsername(string userId, ChangeUsernameRequest request)
    {
        var result = await _userService.ChangeUsername(userId, request.NewUsername);
        return result.ToActionResult();
    }
}
