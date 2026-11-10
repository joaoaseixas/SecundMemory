using Microsoft.Extensions.DependencyInjection;
using SecundMemory.Application.Capacity;
using SecundMemory.Application.Notes;
using SecundMemory.Application.ProfileFacts;

namespace SecundMemory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICapacityService, CapacityService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IProfileFactService, ProfileFactService>();
        return services;
    }
}
