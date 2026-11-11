namespace SecundMemory.Api.Extensions;

/// <summary>
/// Named for the role (API documentation), not the literal library — this
/// project uses .NET's built-in Microsoft.AspNetCore.OpenApi rather than
/// Swashbuckle, so there's one fewer dependency to track.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}
