using System.Globalization;
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
            _log.Information("Request succes");
            await _next(context);
        }
        catch (System.Exception ex)
        {
            // Log ex.Message
            _log.Error("Request faild: " + ex.Message);
            HandleException(context, ex);
        }

    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "text/json";
        // context.Response.StatusCode = 500;
        return context.Response.WriteAsync("ERROR: " + ex.Message);
    }
}

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app,Serilog.ILogger logger)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>(logger);
    }
}