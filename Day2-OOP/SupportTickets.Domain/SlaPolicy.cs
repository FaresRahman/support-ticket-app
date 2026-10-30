namespace SupportTickets.Domain;

/// <summary>Struct (value type): a tiny immutable value with no identity.</summary>
public readonly record struct SlaPolicy(TimeSpan ResponseWindow)
{
    public static SlaPolicy For(Priority priority) => priority switch
    {
        Priority.Critical => new SlaPolicy(TimeSpan.FromHours(4)),
        Priority.High => new SlaPolicy(TimeSpan.FromHours(24)),
        Priority.Medium => new SlaPolicy(TimeSpan.FromHours(72)),
        Priority.Low => new SlaPolicy(TimeSpan.FromHours(168)),
        _ => throw new ArgumentOutOfRangeException(nameof(priority))
    };
}
