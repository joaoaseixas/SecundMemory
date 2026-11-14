using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Pages;
using SecundMemory.Infrastructure.BackgroundJobs;
using SecundMemory.Infrastructure.Caching;
using SecundMemory.Infrastructure.Identity;
using SecundMemory.Infrastructure.Persistence;
using SecundMemory.Infrastructure.Persistence.Interceptors;
using SecundMemory.Infrastructure.Persistence.Repositories;
using SecundMemory.Infrastructure.Search;
using SecundMemory.Infrastructure.Services;
using SecundMemory.Infrastructure.Storage;

namespace SecundMemory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddIdentity(services, configuration);
        AddStorage(services, configuration);

        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        services.AddScoped<ISearchService, PostgresFullTextSearchService>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ReindexSearchJob>();

        services.AddHostedService<SearchIndexUpdater>();
        services.AddHostedService<CleanupSoftDeletedJob>();

        return services;
    }

    /// <summary>
    /// Hosted Postgres providers (Neon, Render, ...) hand out postgres://user:pass@host/db URLs,
    /// which Npgsql doesn't parse; convert them to a key=value connection string.
    /// </summary>
    public static string NormalizePostgresUrl(string connectionString)
    {
        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);
        var builder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort || uri.Port <= 0 ? 5432 : uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
            SslMode = Npgsql.SslMode.Require,
        };
        return builder.ConnectionString;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SecondBrain")
            ?? throw new InvalidOperationException(
                "Connection string 'SecondBrain' is not configured. Set it via " +
                "`dotnet user-secrets set \"ConnectionStrings:SecondBrain\" \"...\"` " +
                "(local dev) or the ConnectionStrings__SecondBrain environment variable. " +
                "It must point at a Postgres database you own — never commit it to source control.");

        connectionString = NormalizePostgresUrl(connectionString);

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        // Provider choice is config-driven (not just the connection string) so
        // that Api.IntegrationTests's WebApplicationFactory can swap in SQLite
        // before the host builds, rather than trying to remove Npgsql's service
        // registrations after the fact — UseNpgsql()/UseSqlite() each add
        // provider-specific services straight into this IServiceCollection, and
        // once both have run, EF Core refuses to pick between them.
        var useSqliteForTests = configuration.GetValue<bool>("Testing:UseSqlite");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            if (useSqliteForTests)
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString);
            }

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<DispatchDomainEventsInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPageRepository, PageRepository>();
        services.AddScoped<IBlockRepository, BlockRepository>();
    }

    private static void AddIdentity(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
    }

    private static void AddStorage(IServiceCollection services, IConfiguration configuration)
    {
        var useS3 = configuration.GetValue<bool>("S3Storage:Enabled");

        if (useS3)
        {
            services.Configure<S3StorageOptions>(configuration.GetSection(S3StorageOptions.SectionName));
            services.AddSingleton<Amazon.S3.IAmazonS3>(sp =>
            {
                var options = configuration.GetSection(S3StorageOptions.SectionName).Get<S3StorageOptions>() ?? new S3StorageOptions();
                return new Amazon.S3.AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(options.Region));
            });
            services.AddScoped<IFileStorage, S3FileStorage>();
        }
        else
        {
            services.Configure<LocalFileStorageOptions>(configuration.GetSection(LocalFileStorageOptions.SectionName));
            services.AddScoped<IFileStorage, LocalFileStorage>();
        }
    }
}
