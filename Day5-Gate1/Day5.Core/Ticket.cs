namespace Day5.Core;

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
    public TicketStatus Status { get; private set; } = TicketStatus.Open;
    public string? Assignee { get; private set; }
    public DateTime? ClosedUtc { get; private set; }

    public void Assign(string assignee)
    {
        if (string.IsNullOrWhiteSpace(assignee))
        {
            throw new ArgumentException("Assignee is required.", nameof(assignee));
        }

        EnsureNotClosed();
        Assignee = assignee.Trim();
        Status = TicketStatus.InProgress;
    }

    public void Close(DateTime closedUtc)
    {
        EnsureNotClosed();
        if (closedUtc < CreatedUtc)
        {
            throw new ArgumentException("A ticket cannot be closed before it was created.", nameof(closedUtc));
        }

        Status = TicketStatus.Closed;
        ClosedUtc = closedUtc;
    }

    /// <summary>
    /// A ticket is overdue only when strictly MORE than its SLA window has passed.
    /// (The injected defect used &gt;=, flagging tickets that were exactly at the deadline.)
    /// </summary>
    public bool IsOverdue(DateTime nowUtc) =>
        Status != TicketStatus.Closed && nowUtc - CreatedUtc > Sla.For(Priority);

    private void EnsureNotClosed()
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException($"Ticket {Id} is already closed.");
        }
    }
}

public static class Sla
{
    public static TimeSpan For(Priority priority) => priority switch
    {
        Priority.Critical => TimeSpan.FromHours(4),
        Priority.High => TimeSpan.FromHours(24),
        Priority.Medium => TimeSpan.FromHours(72),
        Priority.Low => TimeSpan.FromHours(168),
        _ => throw new ArgumentOutOfRangeException(nameof(priority))
    };
}
