namespace Day1.TicketConsole;

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>Reference type: copies of a variable share one Ticket instance.</summary>
public class Ticket
{
    public Ticket(int id, string title, Priority priority)
    {
        Id = id;
        Title = title;
        Priority = priority;
    }

    public int Id { get; }
    public string Title { get; }
    public Priority Priority { get; }

    // Nullable reference type: a ticket may have no assignee yet.
    public string? AssignedTo { get; set; }

    public bool IsOpen { get; set; } = true;

    public override string ToString() =>
        $"#{Id} [{Priority}] {Title} ({(IsOpen ? "Open" : "Closed")})";
}

/// <summary>Value type: assigning it to another variable copies all fields.</summary>
public readonly struct TicketStats
{
    public TicketStats(int total, int open, int[] countByPriority, double averageTitleLength)
    {
        Total = total;
        Open = open;
        CountByPriority = countByPriority;
        AverageTitleLength = averageTitleLength;
    }

    public int Total { get; }
    public int Open { get; }
    public int[] CountByPriority { get; }
    public double AverageTitleLength { get; }
}
