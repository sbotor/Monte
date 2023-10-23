namespace Monte;

public interface IAgentContextProvider
{
    ValueTask<AgentContext> GetContext(CancellationToken cancellationToken = default);
}

public class AgentContext
{
    public string Origin { get; }

    public AgentContext(string origin)
    {
        Origin = origin;
    }
}
