using Wolverine.Runtime.Serialization;

namespace Wolverine.Raven.Internals;

public class IncomingMessage
{
    public IncomingMessage() { }

    public IncomingMessage(Envelope envelope) : this(envelope, envelope.GetStatusInternalProperty(), envelope.GetOwnerIdInternalProperty()) { }

    public IncomingMessage(Envelope envelope, EnvelopeStatus status, int ownerId)
    {
        Id = envelope.Id;
        Status = status;
        OwnerId = ownerId;
        ExecutionTime = envelope.ScheduledTime?.ToUniversalTime();
        Attempts = envelope.Attempts;
        Body = EnvelopeSerializer.Serialize(envelope);
        MessageType = envelope.MessageType!;
        ReceivedAt = envelope.Destination?.ToString();
    }

    public Guid Id { get; set; }
    public EnvelopeStatus Status { get; set; } = EnvelopeStatus.Incoming;
    public int OwnerId { get; set; }
    public DateTimeOffset? ExecutionTime { get; set; }
    public int Attempts { get; set; }
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public string MessageType { get; set; } = string.Empty;
    public string? ReceivedAt { get; set; }
}
