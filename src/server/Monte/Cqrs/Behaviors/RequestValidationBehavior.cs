using FluentValidation;
using MediatR;
using Monte.Models;
using Monte.Models.Exceptions;

namespace Monte.Cqrs.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IValidator<TRequest>[] _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToArray();
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var tasks = _validators.Select(x => x.ValidateAsync(request, cancellationToken));

        var results = await Task.WhenAll(tasks);

        var errors = results.Where(x => !x.IsValid)
            .SelectMany(x => x.Errors.Select(y => (y.PropertyName, new ErrorMessage(y.ErrorCode, y.ErrorMessage))))
            .GroupBy(x => x.PropertyName)
            .Select(x => new ErrorModel(x.Key, x.Select(y => y.Item2)))
            .ToArray();

        if (errors.Length > 0)
        {
            throw new RequestValidationException(errors);
        }

        return await next();
    }
}