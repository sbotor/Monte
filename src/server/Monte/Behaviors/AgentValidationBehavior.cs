using System.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Contracts;
using Monte.Models.Exceptions;
using Monte.Services;

namespace Monte.Behaviors;

public class AgentValidationBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly MonteDbContext _dbContext;
    private readonly IAgentContextProvider _agentContextProvider;

    public AgentValidationBehavior(MonteDbContext dbContext, IAgentContextProvider agentContextProvider)
    {
        _dbContext = dbContext;
        _agentContextProvider = agentContextProvider;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        if (request is not IMetricsKeyRequest metricsKeyRequest)
        {
            return await next();
        }
        
        var agentContext = await _agentContextProvider.GetContextOrDefault(cancellationToken);
        if (agentContext?.Id == null)
        {
            return await next();
        }

        var agent = await _dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentContext.Id, cancellationToken)
            ?? throw new NotFoundException();

        if (metricsKeyRequest.MetricsKey != agent.MetricsKey)
        {
            throw new SecurityException();
        }

        return await next();
    }
}
