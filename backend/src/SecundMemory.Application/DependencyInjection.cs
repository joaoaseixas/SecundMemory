using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SecundMemory.Application.Common.Behaviors;
using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));
        RegisterHandlers(services, assembly, typeof(IDomainEventHandler<>));

        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Outermost to innermost: logged and authorized before anything else
        // runs, validated before the handler does real work, timed around
        // the actual handler, saved only once that handler succeeds.
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly, Type openHandlerInterface)
    {
        var registrations = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openHandlerInterface)
                .Select(serviceType => (serviceType, implementationType: t)));

        foreach (var (serviceType, implementationType) in registrations)
        {
            services.AddScoped(serviceType, implementationType);
        }
    }
}
