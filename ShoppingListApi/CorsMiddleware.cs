namespace ShoppingListApi;

public sealed class CorsMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var origin = context.Request.Headers.Origin.FirstOrDefault();

        context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, PUT, POST, PATCH, DELETE, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "x-requested-with, x-signalr-user-agent, content-type");

        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            return;
        }

        await _next(context);
    }
}
