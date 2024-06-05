using Microsoft.Extensions.DependencyInjection;
using Wolverine.Raven.Internals;

namespace Wolverine.Raven;

public static class WolverineRavenExtensions
{
    public static IServiceCollection AddRavenWithWolverineIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IWolverineExtension, RavenBackedPersistence>();
        
        return services;
    }
}
