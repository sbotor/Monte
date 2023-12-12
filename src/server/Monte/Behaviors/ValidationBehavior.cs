using FluentValidation;
using MediatR;

namespace Monte.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IValidator<TRequest>[] _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToArray();
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Length < 1)
        {
            return await next();
        }
        
        var tasks = _validators.Select(x => x.ValidateAsync(request, cancellationToken));
        var results = await Task.WhenAll(tasks);

        var errors = results.Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(x => x.PropertyName)
            .ToDictionary(x => x.Key,
                x => x.Select(y => y.ErrorCode).ToArray());

        if (errors.Count > 0)
        {
            throw new Monte.Models.Exceptions.ValidationException(errors);
        }

        return await next();
    }
}
