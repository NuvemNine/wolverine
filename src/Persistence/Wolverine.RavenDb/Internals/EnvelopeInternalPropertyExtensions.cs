using System.Reflection;

namespace Wolverine.RavenDb.Internals;

/// <summary>
/// Getting to internal properties via reflection until we can determine that we don't need them
/// </summary>
internal static class EnvelopeInternalPropertyExtensions
{
    private static readonly Type EnvelopeType = typeof(Envelope);
    private const BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    
    public static EnvelopeStatus GetStatusInternalProperty(this Envelope envelope) =>
        envelope.GetInternalPropertyValue<EnvelopeStatus>("Status");

    public static int GetOwnerIdInternalProperty(this Envelope envelope) =>
        envelope.GetInternalPropertyValue<int>("OwnerId");
    
    public static void SetOwnerId(this Envelope envelope, int attempts) =>
        GetInternalProperty<int>("OwnerId")!.SetValue(envelope, attempts);

    public static void SetAttempts(this Envelope envelope, int attempts) =>
        GetInternalProperty<int>("Attempts")!.SetValue(envelope, attempts);

    private static T? GetInternalPropertyValue<T>(this Envelope envelope, string propertyName) =>
        (T?)GetInternalProperty<T>(propertyName)!.GetValue(envelope, null);

    private static PropertyInfo? GetInternalProperty<T>(string propertyName) => 
        EnvelopeType.GetProperty(propertyName, PropertyFlags) 
        ?? throw new InvalidOperationException($"Could not find Envelope internal property {propertyName}");
}
