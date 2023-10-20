namespace Monte.WebApi;

public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private UserInfo? _user;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public ValueTask<UserInfo> GetUser()
    {
        _user ??= ExtractUser();
        return new(_user);
    }

    private UserInfo ExtractUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            throw new InvalidOperationException("No HTTP context found.");
        }

        var id = httpContext.User.FindFirst(x => x.Type == "sub")?.Value
            ?? throw new InvalidOperationException("No subject claim found.");

        return new(id);
    }
}