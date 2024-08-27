using Raven.Client.Documents.Session;

namespace Wolverine.RavenDb;

/// <summary>
///     Wrapped messaging outbox for raven
/// </summary>
public interface IRavenDbOutbox : IMessageBus
{
    /// <summary>
    ///     The current raven session for this outbox
    /// </summary>
    IAsyncDocumentSession Session { get; }

    /// <summary>
    ///     Saves outstanding changes in the raven session and flushes outbox messages to the
    ///     sending agents
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SaveChangesAndFlushMessagesAsync(CancellationToken token = default);

    /// <summary>
    ///     Calling this
    ///     method will force the outbox to send out any outstanding messages
    ///     that were captured as part of processing the transaction if you call SaveChangesAsync()
    ///     directly on the DbContext
    /// </summary>
    /// <returns></returns>
    Task FlushOutgoingMessagesAsync();
}
