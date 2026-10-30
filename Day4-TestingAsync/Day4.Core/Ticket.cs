namespace Day4.Core;

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

public enum TicketStatus
{
    Open,
    Closed
}

public class Ticket
{
    public Ticket(int id, string title, Priority priority, DateTime createdUtc)
    {
        Id = id;
        Title = title;
        Priority = priority;
        CreatedUtc = createdUtc;
    }

    public int Id { get; }
    public string Title { get; }
    public Priority Priority { get; }
    public DateTime CreatedUtc { get; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public DateTime? ClosedUtc { get; set; }
}

public static class Sla
{
    public static TimeSpan For(Priority priority) => priority switch
    {
        Priority.Critical => TimeSpan.FromHours(4),
        Priority.High => TimeSpan.FromHours(24),
        Priority.Medium => TimeSpan.FromHours(72),
        _ => TimeSpan.FromHours(168)
    };
}
