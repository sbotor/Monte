using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;
using Monte.AuthServer.Services;

namespace Monte.AuthServer.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    //private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        //_userManager = userManager;
        _userService = new UserService(userManager);
    }

    [HttpPost("root")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRootAdmin(CreateUserRequest request)
    {
        return await _userService.UserCreation(request, AuthConsts.Roles.MonteAdmin);
    }

    [HttpPost]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        return await _userService.UserCreation(request, AuthConsts.Roles.MonteUser);
    }

    [HttpGet]
    [Authorize(Roles = AuthConsts.Roles.MonteAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        return await _userService.GetUsers();
    }

    [HttpDelete]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string Id)
    {
        return await _userService.DeleteUser(Id);
    }


    [HttpPatch("password")]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword)
    {
        return await _userService.ChangePassword(userId, oldPassword, newPassword);
    }

    [HttpPatch("username")]
    [Authorize(Roles = AuthConsts.RoleGroups.MonteAdminOrUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUsername(string userId, string newUsername)
    {
        return await _userService.ChangeUsername(userId, newUsername);
    }
}
