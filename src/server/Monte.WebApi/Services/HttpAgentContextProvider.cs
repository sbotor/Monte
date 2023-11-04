using Monte.Services;

namespace Monte.WebApi.Services;

public class HttpAgentContextProvider : IAgentContextProvider
{
    private const string AgentIdHeaderName = "Agent-Id";
    
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

        var request = httpContext.Request;
        
        var origin = request.Headers.Origin.FirstOrDefault();
        if (string.IsNullOrEmpty(origin))
        {
            throw new InvalidOperationException("No request Origin found.");
        }

        var agentId = request.Headers[AgentIdHeaderName].FirstOrDefault();
        if (string.IsNullOrEmpty(agentId))
        {
            return new(origin, null);
        }

        if (!Guid.TryParse(agentId, out var parsed))
        {
            throw new InvalidOperationException("Could not parse agent ID.");
        }

        return new(origin, parsed);
    }
}
