using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monte.AuthServer.Features.Users.Extensions;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;

namespace Monte.AuthServer.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> UserCreation(CreateUserRequest request, string role)
    {
        if(role == AuthConsts.Roles.MonteAdmin)
        {
            if (_userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin).Result.Any())
                return new BadRequestObjectResult("Couldn't create admin, becouse an admin already exists in the system.");
        }

        if (string.IsNullOrEmpty(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return new BadRequestObjectResult("Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser is not null)
        {
            return new BadRequestObjectResult("User already exists.");
        }

        var user = new IdentityUser
        {
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new BadRequestObjectResult(result.Errors.ToErrorDict());
        }

        try
        {
            ThrowIfError(await _userManager.AddPasswordAsync(user, request.Password));
            ThrowIfError(await _userManager.AddToRoleAsync(user, role));
        }
        catch (IdentityErrorException e)
        {
            await _userManager.DeleteAsync(user);
            return new BadRequestObjectResult(e.Errors);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
        return new CreatedResult(String.Empty, new UserDetails() { Id = user.Id, Name = user.UserName });
    }
    
    public async Task<IActionResult> ChangeUsername(string userId, string newUsername)
    {
        if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(userId))
        {
            return new BadRequestObjectResult("Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(newUsername);
        if (existingUser is not null)
        {
            return new BadRequestObjectResult("This username is already taken.");
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return new NotFoundObjectResult($"User with the id '{userId}' was not found.");
        }


        try
        {
            ThrowIfError(await _userManager.SetUserNameAsync(usr, newUsername));
        }
        catch (IdentityErrorException e)
        {
            return new BadRequestObjectResult(e.Errors);
        }
        catch
        {
            throw;
        }

        return new OkObjectResult(new UserDetails() { Id = usr.Id, Name = usr.UserName ?? "-" });
    }
    
    public async Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) ||
            string.IsNullOrEmpty(oldPassword) ||
            string.IsNullOrEmpty(newPassword))
        {
            return new BadRequestObjectResult("Invalid credentials.");
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return new NotFoundObjectResult($"User with the id '{userId}' was not found.");
        }

        try
        {
            ThrowIfError(await _userManager.ChangePasswordAsync(usr, oldPassword, newPassword));
        }
        catch (IdentityErrorException e)
        {
            return new BadRequestObjectResult(e.Errors);
        }
        catch
        {
            throw;
        }

        return new NoContentResult();
    }

    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteUser);

        var result = users.Select(x =>
            new UserDetails()
            {
                Id = x.Id,
                Name = x.UserName ?? "-"
            });
        return new OkObjectResult(result);
    }
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var existingUser = await _userManager.FindByIdAsync(userId);
        if (existingUser == null)
        {
            return new NotFoundObjectResult($"User with the id '{userId}' was not found.");
        }
       
        await _userManager.DeleteAsync(existingUser);

        return new NoContentResult();
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


public interface IUserService
{
    public Task<IActionResult> UserCreation(CreateUserRequest request, string role);
    public Task<IActionResult> ChangeUsername(string userId, string newUsername);
    public Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword);
    public Task<IActionResult> GetUsers();
    public Task<IActionResult> DeleteUser(string userId);
}
