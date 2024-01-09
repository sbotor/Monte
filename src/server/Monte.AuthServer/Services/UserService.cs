using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monte.AuthServer.Data;
using Monte.AuthServer.Features.Users.Extensions;
using Monte.AuthServer.Features.Users.Models;
using Monte.AuthServer.Helpers;
using Monte.AuthServer.Models;

namespace Monte.AuthServer.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthDbContext _dbContext;
    private readonly IUserContext _userContext;

    public UserService(
        UserManager<AppUser> userManager,
        AuthDbContext dbContext,
        IUserContext userContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<UserDetails>> CreateUser(CreateUserRequest request, string role)
    {
        if (role == AuthConsts.Roles.MonteAdmin)
        {
            if ((await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin)).Any())
            {
                return Result.Failure<UserDetails>(
                    ErrorType.BadRequest,
                    "Couldn't create admin, because an admin already exists in the system.");
            }
        }

        if (string.IsNullOrEmpty(request.Username)
            || string.IsNullOrEmpty(request.Password))
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest, "Invalid credentials.");
        }

        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser is not null)
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest, "User already exists.");
        }

        var user = new AppUser { UserName = request.Username };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest,
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
            return Result.Failure<UserDetails>(ErrorType.BadRequest, e.Errors);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        var response = new UserDetails { Id = user.Id, Name = user.UserName };

        return Result.Success(response);
    }

    public async Task<Result<UserDetails>> ChangeUsername(string userId, string username)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest, "Invalid credentials.");
        }
        
        var usr = await _userManager.FindByIdAsync(userId);
        if (usr is null)
        {
            return Result.Failure<UserDetails>(ErrorType.NotFound,
                $"User with the id '{userId}' was not found.");
        }

        if (usr.Id != await _userContext.GetUserId() && !await _userContext.IsAdmin())
        {
            return Result.Failure<UserDetails>(ErrorType.Unauthorized);
        }

        var existingUser = await _userManager.FindByNameAsync(username);
        if (existingUser is not null)
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest, "This username is already taken.");
        }

        try
        {
            ThrowIfError(await _userManager.SetUserNameAsync(usr, username));
        }
        catch (IdentityErrorException e)
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest, e.Errors);
        }

        var response = new UserDetails { Id = usr.Id, Name = usr.UserName ?? "-" };

        return Result.Success(response);
    }

    public async Task<Result> ChangePassword(string userId, ChangePasswordRequest request)
    {
        if (string.IsNullOrEmpty(userId) ||
            string.IsNullOrEmpty(request.OldPassword) ||
            string.IsNullOrEmpty(request.NewPassword))
        {
            return Result.Failure(ErrorType.BadRequest,
                "Invalid credentials.");
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return Result.Failure(ErrorType.NotFound,
                $"User with the id '{userId}' was not found.");
        }
        
        if (usr.Id != await _userContext.GetUserId() && !await _userContext.IsAdmin())
        {
            return Result.Failure<UserDetails>(ErrorType.Unauthorized);
        }

        try
        {
            ThrowIfError(await _userManager.ChangePasswordAsync(usr, request.OldPassword, request.NewPassword));
        }
        catch (IdentityErrorException e)
        {
            return Result.Failure(ErrorType.BadRequest, e.Errors.ToString());
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
            return Result.Failure(ErrorType.NotFound, $"User with the id '{userId}' was not found.");
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

public interface IUserService
{
    public Task<Result<UserDetails>> CreateUser(CreateUserRequest request, string role);
    public Task<Result<UserDetails>> ChangeUsername(string userId, string username);
    public Task<Result> ChangePassword(string userId, ChangePasswordRequest request);
    public Task<Result<UserDetails[]>> GetUsers(CancellationToken cancellationToken = default);
    public Task<Result> DeleteUser(string userId);
}
