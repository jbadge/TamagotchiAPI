using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class VisitorIdMiddleware
{
    private readonly RequestDelegate _next;

    public VisitorIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip middleware for Swagger routes
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // Enforce visitor ID header
        if (!context.Request.Headers.ContainsKey("x-visitor-id") ||
            string.IsNullOrWhiteSpace(context.Request.Headers["x-visitor-id"]))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing x-visitor-id header");
            return;
        }

        await _next(context);
    }
}
