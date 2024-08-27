using Microsoft.Extensions.DependencyInjection;
using Wolverine.Persistence.Sagas;

namespace Wolverine.RavenDb.Internals;

/// <summary>
///     Add to your Wolverine application to opt into EF Core-backed
///     transaction and saga persistence middleware.
///     Warning! This has to be used in conjunction with a Wolverine
///     database package
/// </summary>
internal class RavenDbBackedPersistence : IWolverineExtension
{
    public void Configure(WolverineOptions options)
    {
        options.CodeGeneration.AddPersistenceStrategy<RavenDbPersistenceFrameProvider>();

        options.Services.AddScoped(typeof(IRavenDbOutbox), typeof(RavenDbOutbox));
    }
}
