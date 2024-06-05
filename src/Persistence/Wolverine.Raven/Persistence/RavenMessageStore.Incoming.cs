using Raven.Client.Documents.Commands.Batches;
using Raven.Client.Documents.Operations;
using Wolverine.Raven.Internals;
using Wolverine.Transports;

namespace Wolverine.Raven.Persistence;

public partial class RavenMessageStore
{
    public async Task ScheduleExecutionAsync(Envelope envelope)
    {
        await PatchIncomingAsync(envelope,@$"
            this.ScheduledTime = {envelope.ScheduledTime};
            this.Status = {nameof(EnvelopeStatus.Scheduled)};
            this[@metadata][@refresh] = {envelope.ScheduledTime};
        ");
    }
    
    public async Task MoveToDeadLetterStorageAsync(Envelope envelope, Exception? exception)
    {
        var incomingId = FormatIncomingId(envelope);
        using var session = store.OpenAsyncSession();
        
        session.Delete(incomingId);
        await session.StoreAsync(new DeadLetterMessage(envelope, exception));

        await session.SaveChangesAsync();
    }

    public async Task IncrementIncomingEnvelopeAttemptsAsync(Envelope envelope)
    {
        var incomingId = FormatIncomingId(envelope);
        using var session = store.OpenAsyncSession();
        
        session.Advanced.Patch<IncomingMessage, int>(incomingId, x => x.Attempts, envelope.Attempts);

        await session.SaveChangesAsync();
    }

    public async Task StoreIncomingAsync(Envelope envelope) => 
        await StoreIncomingAsync(new IncomingMessage(envelope));

    public async Task StoreIncomingAsync(IReadOnlyList<Envelope> envelopes)
    {
        using var session = store.OpenAsyncSession();

        foreach (var envelope in envelopes) await session.StoreAsync(new IncomingMessage(envelope));

        await session.SaveChangesAsync();
    }

    public Task ScheduleJobAsync(Envelope envelope) => 
        StoreIncomingAsync(new IncomingMessage(envelope, EnvelopeStatus.Scheduled, TransportConstants.AnyNode));

    public async Task MarkIncomingEnvelopeAsHandledAsync(Envelope envelope)
    {
        var expires = DateTimeOffset.UtcNow.Add(durabilitySettings.KeepAfterMessageHandling);

        await PatchIncomingAsync(envelope, @$"
            this.Status = {nameof(EnvelopeStatus.Handled)};
            this[@metadata][@expires] = {expires};
        ");
    }

    public async Task ReleaseIncomingAsync(int ownerId)
    {
        await PatchByQuery($@"from IncomingMessages as m
            where m.OwnerId = {ownerId}
            update
            {{
                this.OwnerId = 0;
            }}"
        );
    }
    
    public async Task ReleaseIncomingAsync(int ownerId, Uri receivedAt)
    {
        await PatchByQuery($@"from IncomingMessages as m
            where m.OwnerId = {ownerId} AND m.ReceivedAt = {receivedAt}
            update
            {{
                this.OwnerId = 0;
            }}"
        );
    }

    private async Task StoreIncomingAsync(IncomingMessage incomingMessage)
    {
        using var session = store.OpenAsyncSession();

        await session.StoreAsync(incomingMessage);

        await session.SaveChangesAsync();
    }

    private async Task PatchByQuery(string queryToUpdate) =>
        await store.Operations.SendAsync(new PatchByQueryOperation(queryToUpdate));

    private async Task PatchIncomingAsync(Envelope envelope, string script)
    {
        var incomingId = FormatIncomingId(envelope);
        using var session = store.OpenAsyncSession();

        session.Advanced.Defer(new PatchCommandData(incomingId, null, new PatchRequest
        {
            Script = script
        }));

        await session.SaveChangesAsync();
    }

    private string IncomingPrefix { get; }

    private string FormatIncomingId(Envelope envelope) => 
        $"{IncomingPrefix}{envelope.Id}";
}
