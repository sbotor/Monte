using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monte.AuthServer.Features.Users.Extensions;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;

namespace Monte.AuthServer.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("root")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRootAdmin(CreateUserRequest request)
    {
        if (_userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin).Result.Any())
            return BadRequest("Cannot create admin, becouse an admin already exists in the system.");

        return await UserCreation(request, AuthConsts.Roles.MonteAdmin);
    }

    [HttpPost("user")]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        return await UserCreation(request, AuthConsts.Roles.MonteUser);
    }

    [HttpGet]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteUser);

        var result = users.Select(x => 
            new UserDetails() 
            { 
                Id = x.Id, 
                Name = x.UserName ?? "-" 
            });
        return Ok(result);
    }

    [HttpDelete]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string Id)
    {
        var existingUser = await _userManager.FindByIdAsync(Id);
        if (existingUser == null)
        {
            return NotFound($"User with the id '{Id}' was not found.");
        }

        await _userManager.DeleteAsync(existingUser);

        return NoContent();
    }


    [HttpPatch("password")]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || 
            string.IsNullOrEmpty(oldPassword) || 
            string.IsNullOrEmpty(newPassword))
        {
            return BadRequest("Invalid credentials.");
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if(usr == null)
        {
            return NotFound($"User with the id '{userId}' was not found.");
        }

        try
        {
            ThrowIfError(await _userManager.ChangePasswordAsync(usr, oldPassword, newPassword));
        }
        catch (IdentityErrorException e)
        {
            return BadRequest(e.Errors);
        }
        catch
        {
            throw;
        }

        return NoContent();
    }

    [HttpPatch("username")]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeUsername(string userId, string newUsername)
    {
        if(string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(userId))
        {
            return BadRequest("Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(newUsername);
        if (existingUser is not null)
        {
            return BadRequest("This username is already taken.");
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return NotFound($"User with the id '{userId}' was not found.");
        }


        try
        {
            ThrowIfError(await _userManager.SetUserNameAsync(usr, newUsername));
        }
        catch (IdentityErrorException e)
        {
            return BadRequest(e.Errors);
        }
        catch
        {
            throw;
        }

        return Ok(new UserDetails() {Id = usr.Id, Name = usr.UserName ?? "-" });
    }

    private async Task<IActionResult> UserCreation(CreateUserRequest request, string role)
    {
        if (string.IsNullOrEmpty(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser is not null)
        {
            return BadRequest("User already exists.");
        }

        var user = new IdentityUser
        {
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.ToErrorDict());
        }

        try
        {
            ThrowIfError(await _userManager.AddPasswordAsync(user, request.Password));
            ThrowIfError(await _userManager.AddToRoleAsync(user, role));
        }
        catch (IdentityErrorException e)
        {
            await _userManager.DeleteAsync(user);
            return BadRequest(e.Errors);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
        return Created(Request.Path, new UserDetails() { Id = user.Id, Name = user.UserName});
    }


    private static void ThrowIfError(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new IdentityErrorException(result.Errors.ToErrorDict());
    }

    private sealed class IdentityErrorException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public IdentityErrorException(IReadOnlyDictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
}
