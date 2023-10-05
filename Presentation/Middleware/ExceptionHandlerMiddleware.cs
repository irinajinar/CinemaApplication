using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using Serilog;

namespace CinemaApplication.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (MultiValidationException multiValidationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity; // 422
            context.Response.ContentType = "application/json";

            var response = new ExceptionResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "Validation errors occurred.",
                ValidationErrors = multiValidationException.ValidationErrors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
            context.Response.ContentType = "application/json";

            var response = new ExceptionResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred. Please try again later."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}