using Wolverine.Runtime.Serialization;

namespace Wolverine.Raven.Internals;

public class DeadLetterMessage
{
    public DeadLetterMessage() { }

    public DeadLetterMessage(Envelope envelope, Exception? ex)
    {
        Id = envelope.Id;
        ExecutionTime = envelope.ScheduledTime?.ToUniversalTime();
        Body = EnvelopeSerializer.Serialize(envelope);
        MessageType = envelope.MessageType!;
        ReceivedAt = envelope.Destination?.ToString();
        ExceptionType = ex?.GetType().ToString();
        ExceptionMessage = ex?.Message;
    }

    public Guid Id { get; set; }
    public DateTimeOffset? ExecutionTime { get; set; }
    public byte[] Body { get; set; } = Array.Empty<byte>();
    public string MessageType { get; set; } = string.Empty;
    public string? ReceivedAt { get; set; }
    public string Source { get; set; }
    public string? ExceptionType { get; set; }
    public string? ExceptionMessage { get; set; }
}
