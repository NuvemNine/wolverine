using Raven.Client.Documents;
using Wolverine.Persistence.Durability;
using Wolverine.Runtime;
using Wolverine.Runtime.Agents;

namespace Wolverine.RavenDb.Internals;

public partial class RavenDbMessageStore : IMessageStore, IMessageInbox, IMessageOutbox
{
    private readonly IDocumentStore store;
    private readonly DurabilitySettings durabilitySettings;

    public RavenDbMessageStore(IDocumentStore store, DurabilitySettings durabilitySettings)
    {
        this.store = store;
        this.durabilitySettings = durabilitySettings;

        IncomingPrefix = FormatPrefix<IncomingMessage>(store);
        OutgoingPrefix = FormatPrefix<OutgoingMessage>(store);
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync(IWolverineRuntime runtime)
    {
        throw new NotImplementedException();
    }

    public void Initialize(IWolverineRuntime runtime)
    {
        throw new NotImplementedException();
    }

    public void Describe(TextWriter writer)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorReport?> LoadDeadLetterEnvelopeAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DrainAsync()
    {
        throw new NotImplementedException();
    }

    public IAgent StartScheduledJobs(IWolverineRuntime runtime)
    {
        throw new NotImplementedException();
    }

    public bool HasDisposed { get; }
    public IMessageInbox Inbox => this;
    public IMessageOutbox Outbox => this;
    public INodeAgentPersistence Nodes { get; }
    public IMessageStoreAdmin Admin { get; }
    public IDeadLetters DeadLetters { get; }

    private static string FormatPrefix<T>(IDocumentStore store)
    {
        var collectionName = store.Conventions.GetCollectionName(typeof(T));
        var prefix = store.Conventions.TransformTypeCollectionNameToDocumentIdPrefix(collectionName);
        var separator = store.Conventions.IdentityPartsSeparator;
        return $"{prefix}{separator}";
    }
}
