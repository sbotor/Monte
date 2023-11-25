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

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
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
            ThrowIfError(await _userManager.AddToRoleAsync(user, AuthConsts.Roles.MonteAdmin));
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

        return Ok();
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
