using Monte.Models.Exceptions;

namespace Monte.Services;

public interface IAgentContextProvider
{
    ValueTask<AgentContext> GetContext(CancellationToken cancellationToken = default);
}

public class AgentContext
{
    public string Origin { get; }
    public Guid? Id { get; }
    
    public Guid RequiredId => Id ?? throw new BadRequestException("No agent ID found.");

    public AgentContext(string origin, Guid? id)
    {
        Origin = origin;
        Id = id;
    }
}
