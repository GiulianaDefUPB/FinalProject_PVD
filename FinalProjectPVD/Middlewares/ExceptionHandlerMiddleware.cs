using System.Globalization;
using System.Net;
using System.Text.Json;

namespace UPB.FinalProjectPVD.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {Message}", ex.Message);
            await HandleException(context, ex);
        }

    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        var response = new { error = ex.Message };
        var json = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}