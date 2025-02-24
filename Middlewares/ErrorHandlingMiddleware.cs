using Hermes.Common;
using System.Net;
using System.Text.Json;

namespace Hermes.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
    int statusCode;
    string errorMessage;

    if (exception is HermesException hermesException)
    {
        statusCode = hermesException.StatusCode; // Set the status code from the exception.
        errorMessage = hermesException.Message;
    }
    else
    {
        statusCode = (int)HttpStatusCode.InternalServerError;
        errorMessage = "An unexpected error occurred.";
    }

    var errorResponse = new
    {
        StatusCode = statusCode,
        Message = errorMessage
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode; // Ensure the status code is set here.

    var jsonResponse = JsonSerializer.Serialize(errorResponse);
    return context.Response.WriteAsync(jsonResponse);
    }

    }
}