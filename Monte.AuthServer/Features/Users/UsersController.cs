using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monte.AuthServer.Features.Users.Extensions;
using Monte.AuthServer.Features.Users.Models;

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
    
    [HttpGet("~/connect/user-info")]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user is null)
        {
            return NotFound();
        }

        var result = new UserDetails
        {
            Id = user.Id,
            Name = user.UserName ?? string.Empty
        };

        return Ok(result);
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
            result = await _userManager.AddPasswordAsync(user, request.Password);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        if (result.Succeeded)
        {
            return Ok();
        }
        
        await _userManager.DeleteAsync(user);
        return BadRequest(result.Errors.ToErrorDict());
    }
}