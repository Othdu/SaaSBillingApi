using System.Net;
using System.Text.Json;
using SaaSBillingApi.Domain.Exceptions;

namespace SaaSBillingApi.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {

            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {

       var (statusCode, message) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            DomainException => (HttpStatusCode.Conflict, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
       if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "An unexpected error occurred.");
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));




    }

}