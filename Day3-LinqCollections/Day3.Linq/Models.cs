namespace Day3.Linq;

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
    InProgress,
    Closed
}

public record Ticket(
    int Id,
    string Title,
    Priority Priority,
    TicketStatus Status,
    string? Assignee,
    string Category,
    DateTime CreatedUtc,
    DateTime? ClosedUtc);

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

public static class SampleData
{
    private static DateTime Day(int day, int hour = 0) => new(2026, 10, day, hour, 0, 0, DateTimeKind.Utc);

    public static readonly DateTime Now = Day(13);

    public static List<Ticket> Tickets() => new()
    {
        new(1, "Login fails", Priority.High, TicketStatus.Open, "Alice", "Auth", Day(1), null),
        new(2, "Typo on pricing page", Priority.Low, TicketStatus.Closed, "Bob", "Content", Day(2), Day(3)),
        new(3, "Payment gateway timeout", Priority.Critical, TicketStatus.InProgress, "Alice", "Billing", Day(5), null),
        new(4, "Export to CSV broken", Priority.Medium, TicketStatus.Open, null, "Reports", Day(7), null),
        new(5, "Slow dashboard", Priority.Medium, TicketStatus.InProgress, "Bob", "Reports", Day(8), null),
        new(6, "Password reset email missing", Priority.High, TicketStatus.Closed, "Carol", "Auth", Day(9), Day(10, 12)),
        new(7, "Dark mode request", Priority.Low, TicketStatus.Open, null, "UI", Day(10), null),
        new(8, "Invoice totals wrong", Priority.High, TicketStatus.Open, "Carol", "Billing", Day(12), null)
    };
}
