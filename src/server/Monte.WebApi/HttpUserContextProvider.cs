namespace Monte.WebApi;

public class HttpUserContextProvider : IUserContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private UserContext? _user;

    public HttpUserContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public ValueTask<UserContext> GetContext(CancellationToken cancellationToken = default)
    {
        _user ??= ExtractUser();
        return new(_user);
    }

    private UserContext ExtractUser()
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
