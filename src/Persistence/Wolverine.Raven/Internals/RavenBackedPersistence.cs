using Microsoft.Extensions.DependencyInjection;
using Wolverine.Persistence.Sagas;
using Wolverine.Raven.Codegen;

namespace Wolverine.Raven.Internals;

/// <summary>
///     Add to your Wolverine application to opt into EF Core-backed
///     transaction and saga persistence middleware.
///     Warning! This has to be used in conjunction with a Wolverine
///     database package
/// </summary>
internal class RavenBackedPersistence : IWolverineExtension
{
    public void Configure(WolverineOptions options)
    {
        options.CodeGeneration.AddPersistenceStrategy<RavenPersistenceFrameProvider>();

        options.Services.AddScoped(typeof(IRavenOutbox), typeof(RavenOutbox));
    }
}
