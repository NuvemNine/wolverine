using Raven.Client.Documents.Indexes;

namespace Wolverine.RavenDb.Internals;

public partial class RavenDbMessageStore
{
    public async Task<IReadOnlyList<Envelope>> LoadOutgoingAsync(Uri destination)
    {
        using var session = store.OpenAsyncSession();

        var query = session.Query<OutgoingMessage, OutgoingMessageIndex>()
            .Where(x => x.Destination == destination.ToString());

        var stream = await session.Advanced.StreamAsync(query);
        
        var enumerable = new StreamAsyncEnumerable<OutgoingMessage>(stream);

        var list = new List<Envelope>();
        await foreach (var item in enumerable)
        {
            list.Add(item.Document.ToEnvelope());
        }

        return list;
    }
    
    public async Task StoreOutgoingAsync(Envelope envelope, int ownerId)
    {
        using var session = store.OpenAsyncSession();

        await session.StoreAsync(new OutgoingMessage(envelope));

        await session.SaveChangesAsync();
    }

    public async Task DeleteOutgoingAsync(Envelope[] envelopes)
    {
        using var session = store.OpenAsyncSession();

        foreach (var envelope in envelopes)
        {
            var outgoingId = FormatOutgoingId(envelope);
            session.Delete(outgoingId);
        }
        
        await session.SaveChangesAsync();
    }

    public async Task DeleteOutgoingAsync(Envelope envelope)
    {
        using var session = store.OpenAsyncSession();

        var outgoingId = FormatOutgoingId(envelope);
        session.Delete(outgoingId);

        await session.SaveChangesAsync();
    }

    public async Task DiscardAndReassignOutgoingAsync(Envelope[] discards, Envelope[] reassigned, int nodeId)
    {
        using var session = store.OpenAsyncSession();
        
        foreach (var envelope in discards)
        {
            session.Delete(FormatOutgoingId(envelope));
        }

        foreach (var envelope in reassigned)
        {
            session.Advanced.Patch<OutgoingMessage, int>(FormatOutgoingId(envelope), x => x.OwnerId, nodeId);
        }

        await session.SaveChangesAsync();
    }
    
    private string OutgoingPrefix { get; }

    private string FormatOutgoingId(Envelope envelope) =>
        $"{OutgoingPrefix}{envelope.Id}";
}

public class OutgoingMessageIndex : AbstractIndexCreationTask<OutgoingMessage>
{
    public OutgoingMessageIndex()
    {
        Map = docs =>
            from doc in docs
            select new
            {
                doc.Destination,
                doc.MessageType,
                doc.OwnerId
            };
    }
}
