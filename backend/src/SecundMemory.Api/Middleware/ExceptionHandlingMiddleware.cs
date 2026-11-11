using System.Net;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Domain.Exceptions;

namespace SecundMemory.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, body) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, (object)new { title = ex.Message }),
            ValidationException ex => (HttpStatusCode.BadRequest, (object)new { title = "One or more validation errors occurred.", errors = ex.Errors }),
            UnauthorizedAccessException ex => (HttpStatusCode.Unauthorized, (object)new { title = ex.Message }),
            ForbiddenException ex => (HttpStatusCode.Forbidden, (object)new { title = ex.Message }),
            DomainException ex => (HttpStatusCode.BadRequest, (object)new { title = ex.Message }),
            _ => (HttpStatusCode.InternalServerError, (object)new { title = "An unexpected error occurred." }),
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception handling {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(body);
    }
}
