using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monte.AuthServer.Data;
using Monte.AuthServer.Features.Users.Extensions;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;

namespace Monte.AuthServer.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _dbContext;

    public UserService(UserManager<AppUser> userManager, AuthDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<Result<UserDetails>> CreateUser(CreateUserRequest request, string role)
    {
        if (role == AuthConsts.Roles.MonteAdmin)
        {
            if ((await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin)).Any())
            {
                return Result.Failure<UserDetails>(
                    Result.ErrorType.BadRequest,
                    "Couldn't create admin, because an admin already exists in the system.");
            }
        }

        if (string.IsNullOrEmpty(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest, "Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser is not null)
        {
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest, "User already exists.");
        }

        var user = new AppUser { UserName = request.Username };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest,
                result.Errors.ToErrorDictionary());
        }

        try
        {
            ThrowIfError(await _userManager.AddPasswordAsync(user, request.Password));
            ThrowIfError(await _userManager.AddToRoleAsync(user, role));
        }
        catch (IdentityErrorException e)
        {
            await _userManager.DeleteAsync(user);
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest, e.Errors);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        var response = new UserDetails { Id = user.Id, Name = user.UserName };

        return Result.Success(response);
    }

    public async Task<Result<UserDetails>> ChangeUsername(ChangeUsernameRequest request)
    {
        if (string.IsNullOrEmpty(request.NewUsername) || string.IsNullOrEmpty(request.UserId))
        {
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest, "Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(request.NewUsername);
        if (existingUser is not null)
        {
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest, "This username is already taken.");
        }

        var usr = await _userManager.FindByIdAsync(request.UserId);
        if (usr == null)
        {
            return Result.Failure<UserDetails>(Result.ErrorType.NotFound,
                $"User with the id '{request.UserId}' was not found.");
        }

        try
        {
            ThrowIfError(await _userManager.SetUserNameAsync(usr, request.NewUsername));
        }
        catch (IdentityErrorException e)
        {
            return Result.Failure<UserDetails>(Result.ErrorType.BadRequest, e.Errors);
        }

        var response = new UserDetails { Id = usr.Id, Name = usr.UserName ?? "-" };

        return Result.Success(response);
    }

    public async Task<Result> ChangePassword(ChangePasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId) ||
            string.IsNullOrEmpty(request.OldPassword) ||
            string.IsNullOrEmpty(request.NewPassword))
        {
            return Result.Failure(Result.ErrorType.BadRequest,
                "Invalid credentials.");
        }

        var usr = await _userManager.FindByIdAsync(request.UserId);
        if (usr == null)
        {
            return Result.Failure(Result.ErrorType.NotFound,
                $"User with the id '{request.UserId}' was not found.");
        }

        try
        {
            ThrowIfError(await _userManager.ChangePasswordAsync(usr, request.OldPassword, request.NewPassword));
        }
        catch (IdentityErrorException e)
        {
            return Result.Failure(Result.ErrorType.BadRequest, e.Errors.ToString());
        }

        return Result.Success();
    }

    public async Task<Result<UserDetails[]>> GetUsers(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users.AsNoTracking()
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .OrderBy(x => x.UserName)
            .Select(x => new UserDetails
            {
                Id = x.Id,
                Name = x.UserName ?? "-",
                Role = x.UserRoles.FirstOrDefault()!.Role.Name!
            })
            .ToArrayAsync(cancellationToken);

        return Result.Success(users);
    }

    public async Task<Result> DeleteUser(string userId)
    {
        var existingUser = await _userManager.FindByIdAsync(userId);
        if (existingUser == null)
        {
            return Result.Failure(Result.ErrorType.NotFound, $"User with the id '{userId}' was not found.");
        }

        await _userManager.DeleteAsync(existingUser);

        return Result.Success();
    }

    private static void ThrowIfError(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new IdentityErrorException(result.Errors.ToErrorDictionary());
    }

    private sealed class IdentityErrorException : Exception
    {
        public ErrorDictionary Errors { get; }

        public IdentityErrorException(ErrorDictionary errors)
        {
            Errors = errors;
        }
    }
}

public class Result
{
    private static readonly Result SuccessfulResult = new();

    public string? ErrorMessage { get; }
    public ErrorType ErrType { get; }

    protected Result(string? error, ErrorType type)
    {
        ErrorMessage = error;
        ErrType = type;
    }

    protected Result()
    {
        ErrorMessage = string.Empty;
        ErrType = ErrorType.None;
    }

    public enum ErrorType
    {
        None, NotFound, BadRequest
    }

    public static Result Success()
        => SuccessfulResult;

    public static Result Failure(ErrorType error, string? message = null)
        => new(message, error);

    public static Result<T> Success<T>(T? value)
        => Result<T>.Success(value);

    public static Result<T> Failure<T>(ErrorType error, string? message = null)
        => Result<T>.Failure(error, message);
}

public class Result<T> : Result
{
    public T? Object { get; }

    protected Result(T? obj)
    {
        Object = obj;
    }

    protected Result(string? error, ErrorType type) : base(error, type)
    {
    }

    public static Result<T> Success(T? value)
        => new(value);

    public static new Result<T> Failure(ErrorType error, string? message = null)
        => new(message, error);
}

public interface IUserService
{
    public Task<Result<UserDetails>> CreateUser(CreateUserRequest request, string role);
    public Task<Result<UserDetails>> ChangeUsername(ChangeUsernameRequest request);
    public Task<Result> ChangePassword(ChangePasswordRequest request);
    public Task<Result<UserDetails[]>> GetUsers(CancellationToken cancellationToken = default);
    public Task<Result> DeleteUser(string userId);
}
