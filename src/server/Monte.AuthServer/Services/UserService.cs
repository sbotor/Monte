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

    public async Task<Result> EnsureAdmin(string username, string password)
    {
        var admins = await _userManager.GetUsersInRoleAsync(AuthConsts.Roles.MonteAdmin);
        if (admins.Count > 0)
        {
            return Result.Success();
        }

        return await CreateUserCore(new() { Username = username, Password = password },
            AuthConsts.Roles.MonteAdmin);
    }

    public async Task<Result<UserDetails>> CreateUser(CreateUserRequest request)
        => await CreateUserCore(request, AuthConsts.Roles.MonteUser);

    public async Task<Result<UserDetails>> ChangeUsername(string userId, string username)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest, "Invalid credentials.");
        }

        if (userId != await _userContext.GetUserId()
            && !await _userContext.IsAdmin())
        {
            return Result.Failure<UserDetails>(ErrorType.Forbidden);
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr is null)
        {
            return Result.Failure<UserDetails>(ErrorType.NotFound,
                $"User with the id '{userId}' was not found.");
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

        if (userId != await _userContext.GetUserId()
            && !await _userContext.IsAdmin())
        {
            return Result.Failure<UserDetails>(ErrorType.Forbidden);
        }

        var usr = await _userManager.FindByIdAsync(userId);
        if (usr == null)
        {
            return Result.Failure(ErrorType.NotFound,
                $"User with the id '{userId}' was not found.");
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
            .Select(x => new UserDetails { Id = x.Id, Name = x.UserName ?? "-", Role = x.UserRoles.First().Role.Name! })
            .ToArrayAsync(cancellationToken);

        return Result.Success(users);
    }

    public async Task<Result<UserDetails>> GetUser(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
        {
            return Result.Failure<UserDetails>(ErrorType.BadRequest);
        }

        if (id != await _userContext.GetUserId(cancellationToken)
            && !await _userContext.IsAdmin(cancellationToken))
        {
            return Result.Failure<UserDetails>(ErrorType.Forbidden);
        }

        var user = await _dbContext.Users.AsNoTracking()
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .Select(x => new UserDetails { Id = x.Id, Name = x.UserName!, Role = x.UserRoles.First().Role.Name! })
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return user is null
            ? Result.Failure<UserDetails>(ErrorType.NotFound)
            : Result.Success(user);
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

    private async Task<Result<UserDetails>> CreateUserCore(CreateUserRequest request, string role)
    {
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
    Task<Result> EnsureAdmin(string username, string password);
    public Task<Result<UserDetails>> CreateUser(CreateUserRequest request);
    public Task<Result<UserDetails>> ChangeUsername(string userId, string username);
    public Task<Result> ChangePassword(string userId, ChangePasswordRequest request);
    public Task<Result<UserDetails[]>> GetUsers(CancellationToken cancellationToken = default);
    public Task<Result> DeleteUser(string userId);
    Task<Result<UserDetails>> GetUser(string id, CancellationToken cancellationToken = default);
}
