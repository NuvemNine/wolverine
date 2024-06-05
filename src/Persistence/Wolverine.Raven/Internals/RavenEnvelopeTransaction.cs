using Raven.Client.Documents.Session;
using Wolverine.Persistence.Durability;

namespace Wolverine.Raven.Internals;

internal class RavenEnvelopeTransaction : IEnvelopeTransaction
{
    public RavenEnvelopeTransaction(IAsyncDocumentSession session)
    {
        Session = session;
    }

    public IAsyncDocumentSession Session { get; }

    public async Task PersistOutgoingAsync(Envelope envelope) => 
        await Session.StoreAsync(new OutgoingMessage(envelope));

    public async Task PersistOutgoingAsync(Envelope[] envelopes)
    {
        foreach (var envelope in envelopes)
        {
            var outgoing = new OutgoingMessage(envelope);
            await Session.StoreAsync(outgoing);
        }
    }

    public async Task PersistIncomingAsync(Envelope envelope) => 
        await Session.StoreAsync(new IncomingMessage(envelope));

    public ValueTask RollbackAsync()
    {
        //This is not needed.
        //Temporarily adding this to see if saveChanges would still be called
        Session.Advanced.Clear();
        return ValueTask.CompletedTask;
    }
}
