using System.Security.Claims;
using Monte.AuthServer.Helpers;
using OpenIddict.Abstractions;

namespace Monte.AuthServer.Services;

public interface IUserContext
{
    ValueTask<string> GetUserId(CancellationToken cancellationToken = default);
    ValueTask<bool> IsAdmin(CancellationToken cancellationToken = default);
}

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    private string? _userId;
    private string? _role;
    
    public ValueTask<string> GetUserId(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_userId))
        {
            return ValueTask.FromResult(_userId);
        }

        var id = GetClaim(OpenIddictConstants.Claims.Subject);
        if (string.IsNullOrEmpty(id))
        {
            throw new InvalidOperationException("No user ID found.");
        }

        _userId = id;

        return ValueTask.FromResult(_userId);
    }

    public ValueTask<bool> IsAdmin(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_role))
        {
            return ValueTask.FromResult(_role == AuthConsts.Roles.MonteAdmin);
        }

        var role = GetClaim(OpenIddictConstants.Claims.Role);
        if (string.IsNullOrEmpty(role))
        {
            throw new InvalidOperationException("No role found.");
        }

        _role = role;

        return ValueTask.FromResult(_role == AuthConsts.Roles.MonteAdmin);
    }

    private string? GetClaim(string name)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            throw new InvalidOperationException("No HTTP context found.");
        }

        return httpContext.User.FindFirstValue(name);
    }
}
