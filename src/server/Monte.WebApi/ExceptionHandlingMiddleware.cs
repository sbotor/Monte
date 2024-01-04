using System.Net;
using System.Security;
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
            var resp = context.Response;
            Task responseTask;
            HttpStatusCode code;
            
            switch (exception)
            {
                case NotFoundException notFound:
                    code = HttpStatusCode.NotFound;
                    responseTask = resp.WriteAsJsonAsync(notFound.Message);
                    break;
                case ValidationException validation:
                    code = HttpStatusCode.BadRequest;
                    responseTask = resp.WriteAsJsonAsync(validation.ErrorCodes);
                    break;
                case BadRequestException badRequest:
                    code = HttpStatusCode.BadRequest;
                    responseTask = resp.WriteAsJsonAsync(badRequest.Message);
                    break;
                case SecurityException security:
                    code = HttpStatusCode.Forbidden;
                    responseTask = resp.WriteAsJsonAsync(security.Message);
                    break;
                default:
                    throw;
            }

            context.Response.StatusCode = (int)code;
            await responseTask;
        }
    }
}
