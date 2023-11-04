using System.Net;
using Monte.Models.Exceptions;

namespace Monte.WebApi;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            string? body;
            HttpStatusCode code;
            
            switch (exception)
            {
                case NotFoundException notFound:
                    code = HttpStatusCode.NotFound;
                    body = notFound.Message;
                    break;
                case BadRequestException badRequest:
                    code = HttpStatusCode.BadRequest;
                    body = badRequest.Message;
                    break;
                default:
                    throw;
            }

            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsJsonAsync(body);
        }
    }
}
