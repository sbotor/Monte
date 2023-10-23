namespace Monte.WebApi;

public class HttpAgentContextProvider : IAgentContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private AgentContext? _agent;

    public HttpAgentContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ValueTask<AgentContext> GetContext(CancellationToken cancellationToken = default)
    {
        _agent ??= ExtractContext();
        return new(_agent);
    }

    private AgentContext ExtractContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            throw new InvalidOperationException("No HTTP context found.");
        }

        var origin = httpContext.Request.Headers.Origin.FirstOrDefault();

        if (string.IsNullOrEmpty(origin))
        {
            throw new InvalidOperationException("No request Origin found.");
        }

        return new(origin);
    }
}
