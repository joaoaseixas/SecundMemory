namespace SecundMemory.Api.Extensions;

/// <summary>
/// Off by default: the Angular dev server proxies /api (see
/// frontend/secund-memory-web/proxy.conf.json) and production deploys the
/// built Angular app from the same origin as this API, so there's normally
/// no cross-origin request to allow. Configure AllowedOrigins in
/// appsettings if the frontend is ever hosted on a different origin.
/// </summary>
public static class CorsExtensions
{
    private const string PolicyName = "Configured";

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(PolicyName, policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            }
        }));

        return services;
    }

    public static WebApplication UseConfiguredCors(this WebApplication app, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        if (allowedOrigins.Length > 0)
        {
            app.UseCors(PolicyName);
        }

        return app;
    }
}
