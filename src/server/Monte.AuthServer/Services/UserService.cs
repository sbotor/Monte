using Microsoft.AspNetCore.Identity;
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

    public async Task<Result> CreateUser(CreateUserRequest request, string role)
    {
        if(role == AuthConsts.Roles.MonteAdmin)
        {
            if ((await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin)).Any())
                return new Result("Couldn't create admin, becouse an admin already exists in the system.", Result.ErrorType.BadRequest);
        }

        if (string.IsNullOrEmpty(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return new Result("Invalid credentials.", Result.ErrorType.BadRequest);
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser is not null)
        {
            return new Result("User already exists.", Result.ErrorType.BadRequest);
        }

        var user = new IdentityUser
        {
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new Result(result.Errors.ToErrorDict().ToString(), Result.ErrorType.BadRequest);
        }

        try
        {
            ThrowIfError(await _userManager.AddPasswordAsync(user, request.Password));
            ThrowIfError(await _userManager.AddToRoleAsync(user, role));
        }
        catch (IdentityErrorException e)
        {
            await _userManager.DeleteAsync(user);
            return new Result(e.Errors.ToString(), Result.ErrorType.BadRequest);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        var response = new UserDetails() { Id = user.Id, Name = user.UserName };

        return new Result<UserDetails>(response);
    }
    
    public async Task<Result> ChangeUsername(string userId, string newUsername)
    {
        if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(userId))
        {
            return new Result("Invalid credentials.", Result.ErrorType.BadRequest);
        }

        var existingUser = await _userManager.FindByNameAsync(newUsername);
        if (existingUser is not null)
        {
            return new Result("This username is already taken.", Result.ErrorType.BadRequest);
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return new Result($"User with the id '{userId}' was not found.", Result.ErrorType.NotFound);
        }


        try
        {
            ThrowIfError(await _userManager.SetUserNameAsync(usr, newUsername));
        }
        catch (IdentityErrorException e)
        {
            return new Result(e.Errors.ToString(), Result.ErrorType.BadRequest);
        }
        catch
        {
            throw;
        }

        var response = new UserDetails() { Id = usr.Id, Name = usr.UserName ?? "-" };

        return new Result<UserDetails>(response);
    }
    
    public async Task<Result> ChangePassword(string userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(userId) ||
            string.IsNullOrEmpty(oldPassword) ||
            string.IsNullOrEmpty(newPassword))
        {
            return new Result("Invalid credentials.", Result.ErrorType.BadRequest);
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return new Result($"User with the id '{userId}' was not found.", Result.ErrorType.NotFound);
        }

        try
        {
            ThrowIfError(await _userManager.ChangePasswordAsync(usr, oldPassword, newPassword));
        }
        catch (IdentityErrorException e)
        {
            return new Result(e.Errors.ToString(), Result.ErrorType.BadRequest);
        }
        catch
        {
            throw;
        }

        return new Result();
    }

    public async Task<Result> GetUsers()
    {
        var users = await Task.Run(() => _userManager.Users);
        var result = users.Select(x =>
            new UserDetails()
            {
                Id = x.Id,
                Name = x.UserName ?? "-"
            });
        return new Result<IEnumerable<UserDetails>>(result);
    }

    public async Task<Result> DeleteUser(string userId)
    {
        var existingUser = await _userManager.FindByIdAsync(userId);
        if (existingUser == null)
        {
            return new Result($"User with the id '{userId}' was not found.", Result.ErrorType.NotFound);
        }
       
        await _userManager.DeleteAsync(existingUser);

        return new Result();
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

public class Result
{
    public string? ErrorMessage { get; set; }
    public ErrorType ErrType { get; set; }

    public Result(string? Error, ErrorType type)
    {
        ErrorMessage = Error;
        ErrType = type;
    }

    public Result()
    {
        ErrorMessage = string.Empty;
        ErrType = ErrorType.None;
    }

    public enum ErrorType
    {
        None, NotFound, BadRequest
    }
}

public class Result<T> : Result
{
    public T? Object { get; set; }

    public Result(T? Obejct, string? Error = null) : base(Error, ErrorType.None)
    {
        Object = Obejct;
    }
}

public interface IUserService
{
    public Task<Result> CreateUser(CreateUserRequest request, string role);
    public Task<Result> ChangeUsername(string userId, string newUsername);
    public Task<Result> ChangePassword(string userId, string oldPassword, string newPassword);
    public Task<Result> GetUsers();
    public Task<Result> DeleteUser(string userId);
}
