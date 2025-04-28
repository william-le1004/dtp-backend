using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case AggregateException aggregateException:
                problem = new()
                {
                    Title = aggregateException.Message,
                    Status = StatusCodes.Status403Forbidden,
                    Detail = aggregateException.InnerException?.Message,
                    Type = nameof(AggregateException)
                };
                break;
            default:
                problem = new ()
                {
                    Title = exception.Message,
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.StackTrace,
                    Type = nameof(HttpStatusCode.InternalServerError)
                };
                break;
        }

        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem);
    }
}