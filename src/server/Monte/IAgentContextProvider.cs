namespace Monte;

public interface IAgentContextProvider
{
    ValueTask<AgentContext> GetContext(CancellationToken cancellationToken = default);
}

public class AgentContext
{
    public string Origin { get; }
    public Guid? Id { get; }

    public AgentContext(string origin, Guid? id)
    {
        Origin = origin;
        Id = id;
    }
}
