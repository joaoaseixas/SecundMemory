using System.Security.Cryptography;
using System.Text;

namespace SecundMemory.Api.Middleware;

/// <summary>
/// Protects /mcp. MCP clients can't do the interactive JWT login, so the endpoint takes a
/// static key (Mcp:ApiKey, sent as "Authorization: Bearer &lt;key&gt;"). With no key configured
/// the endpoint is only reachable in Development; elsewhere it answers 404.
/// </summary>
public class McpApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/mcp"))
        {
            await next(context);
            return;
        }

        var key = configuration["Mcp:ApiKey"];
        if (string.IsNullOrEmpty(key))
        {
            if (env.IsDevelopment())
            {
                await next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var header = context.Request.Headers.Authorization.ToString();
        var provided = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..] : string.Empty;

        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(key)))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }
}
