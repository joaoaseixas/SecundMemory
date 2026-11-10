using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecundMemory.Application.Common.Interfaces;
using SecundMemory.Infrastructure.Persistence;

namespace SecundMemory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SecondBrain")
            ?? throw new InvalidOperationException(
                "Connection string 'SecondBrain' is not configured. Set it via " +
                "`dotnet user-secrets set \"ConnectionStrings:SecondBrain\" \"...\"` " +
                "(local dev) or the ConnectionStrings__SecondBrain environment variable (any other environment). " +
                "It must point at a Postgres database you own — never commit it to source control.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
