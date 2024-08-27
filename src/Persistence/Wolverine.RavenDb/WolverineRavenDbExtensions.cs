using Microsoft.Extensions.DependencyInjection;
using Wolverine.RavenDb.Internals;

namespace Wolverine.RavenDb;

public static class WolverineRavenDbExtensions
{
    public static IServiceCollection AddRavenWithWolverineIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IWolverineExtension, RavenDbBackedPersistence>();
        
        return services;
    }
}
