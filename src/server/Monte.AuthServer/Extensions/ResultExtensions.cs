using Microsoft.AspNetCore.Mvc;
using Monte.AuthServer.Models;

namespace Monte.AuthServer.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
        => result.ToActionResult(() => new NoContentResult());

    public static IActionResult ToActionResult<T>(this Result<T> result)
        => result.ToActionResult(() => new OkObjectResult(result.Object));
    
    private static IActionResult ToActionResult(this Result result, Func<IActionResult> successFactory)
        => result.ErrType switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(result.ErrorMessage),
            ErrorType.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(result.ErrorMessage),
            ErrorType.None => successFactory(),
            _ => throw new InvalidOperationException()
        };
}
