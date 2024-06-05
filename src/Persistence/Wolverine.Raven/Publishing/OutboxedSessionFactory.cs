using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Wolverine.Raven.Internals;
using Wolverine.Runtime;

namespace Wolverine.Raven.Publishing;

public class OutboxedSessionFactory
{
    private readonly IDocumentStore _store;

    public OutboxedSessionFactory(IDocumentStore store)
    {
        _store = store;
    }

    /// <summary>Build new instances of IDocumentSession on demand</summary>
    /// <returns></returns>
    public IAsyncDocumentSession OpenSession(MessageContext context)
    {
        var session = _store.OpenAsyncSession();

        ConfigureSession(context, session);

        return session;
    }
    
    private void ConfigureSession(MessageContext context, IAsyncDocumentSession session)
    {
        context.EnlistInOutbox(new RavenEnvelopeTransaction(session));
        
        session.Advanced.OnAfterSaveChanges += (_, _) => 
            context.FlushOutgoingMessagesAsync().GetAwaiter().GetResult();
    }
}
