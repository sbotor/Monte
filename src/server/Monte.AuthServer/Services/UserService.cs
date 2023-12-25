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

    public async Task<Result<UserDetails>> CreateUser(CreateUserRequest request, string role)
    {
        if(role == AuthConsts.Roles.MonteAdmin)
        {
            var us = await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin);
            if ((await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin)).Any())
            {
                return new Result<UserDetails>("Couldn't create admin, because an admin already exists in the system.", Result.ErrorType.BadRequest);
            }
        }

        if (string.IsNullOrEmpty(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return new Result<UserDetails>("Invalid credentials.", Result.ErrorType.BadRequest);
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser is not null)
        {
            return new Result<UserDetails>("User already exists.", Result.ErrorType.BadRequest);
        }

        var user = new IdentityUser
        {
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return new Result<UserDetails>(result.Errors.ToErrorDict().ToString(), Result.ErrorType.BadRequest);
        }

        try
        {
            ThrowIfError(await _userManager.AddPasswordAsync(user, request.Password));
            ThrowIfError(await _userManager.AddToRoleAsync(user, role));
        }
        catch (IdentityErrorException e)
        {
            await _userManager.DeleteAsync(user);
            return new Result<UserDetails>(e.Errors.ToString(), Result.ErrorType.BadRequest);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        var response = new UserDetails() { Id = user.Id, Name = user.UserName };

        return new Result<UserDetails>(response);
    }
    
    public async Task<Result<UserDetails>> ChangeUsername(ChangeUsernameRequest request)
    {
        if (string.IsNullOrEmpty(request.NewUsername) || string.IsNullOrEmpty(request.UserId))
        {
            return new Result<UserDetails>("Invalid credentials.", Result.ErrorType.BadRequest);
        }

        var existingUser = await _userManager.FindByNameAsync(request.NewUsername);
        if (existingUser is not null)
        {
            return new Result<UserDetails>("This username is already taken.", Result.ErrorType.BadRequest);
        }

        var usr = await _userManager.FindByIdAsync(request.UserId);
        if (usr == null)
        {
            return new Result<UserDetails>($"User with the id '{request.UserId}' was not found.", Result.ErrorType.NotFound);
        }


        try
        {
            ThrowIfError(await _userManager.SetUserNameAsync(usr, request.NewUsername));
        }
        catch (IdentityErrorException e)
        {
            return new Result<UserDetails>(e.Errors.ToString(), Result.ErrorType.BadRequest);
        }
        catch
        {
            throw;
        }

        var response = new UserDetails() { Id = usr.Id, Name = usr.UserName ?? "-" };

        return new Result<UserDetails>(response);
    }
    
    public async Task<Result> ChangePassword(ChangePasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId) ||
            string.IsNullOrEmpty(request.OldPassword) ||
            string.IsNullOrEmpty(request.NewPassword))
        {
            return new Result("Invalid credentials.", Result.ErrorType.BadRequest);
        }

        var usr = await _userManager.FindByIdAsync(request.UserId);
        if (usr == null)
        {
            return new Result($"User with the id '{request.UserId}' was not found.", Result.ErrorType.NotFound);
        }

        try
        {
            ThrowIfError(await _userManager.ChangePasswordAsync(usr, request.OldPassword, request.NewPassword));
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

    public async Task<Result<IEnumerable<UserDetails>>> GetUsers()
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

    public Result(string? error, ErrorType type)
    {
        ErrorMessage = error;
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

    public Result(T? obj) : base()
    {
        Object = obj;
    }
    public Result(string? error, ErrorType type) : base(error, type) { }
}

public interface IUserService
{
    public Task<Result<UserDetails>> CreateUser(CreateUserRequest request, string role);
    public Task<Result<UserDetails>> ChangeUsername(ChangeUsernameRequest request);
    public Task<Result> ChangePassword(ChangePasswordRequest request);
    public Task<Result<IEnumerable<UserDetails>>> GetUsers();
    public Task<Result> DeleteUser(string userId);
}
