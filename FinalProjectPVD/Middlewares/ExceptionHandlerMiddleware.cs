using System.Globalization;
using System.Net;
using System.Text.Json;
using Serilog;

namespace UPB.FinalProjectPVD.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _log;

    public ExceptionHandlerMiddleware(RequestDelegate next, Serilog.ILogger log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next delegate/middleware in the pipeline.
            _log.Information("Successful request.");
            await _next(context);
        }
        catch (System.Exception ex)
        {
            _log.Error(ex, "An error occurred while processing the request: {Message}", ex.Message);
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
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app,Serilog.ILogger logger)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>(logger);
    }
}