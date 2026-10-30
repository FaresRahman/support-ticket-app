namespace Day5.Core;

public class TicketService
{
    private readonly List<Ticket> _tickets = new();
    private int _nextId = 1;

    public IReadOnlyList<Ticket> All => _tickets;

    public Ticket Create(string title, Priority priority, DateTime createdUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentOutOfRangeException(nameof(priority));
        }

        var ticket = new Ticket(_nextId++, title.Trim(), priority, createdUtc);
        _tickets.Add(ticket);
        return ticket;
    }

    public Ticket Get(int id) =>
        _tickets.FirstOrDefault(t => t.Id == id) ?? throw new KeyNotFoundException($"Ticket {id} not found.");

    public IReadOnlyList<Ticket> GetOverdue(DateTime nowUtc) =>
        _tickets.Where(t => t.IsOverdue(nowUtc)).ToList();

    /// <summary>
    /// LINQ walkthrough query: open tickets grouped by priority, most urgent group first,
    /// each group's count. Priorities with no open tickets are omitted. O(n log k) for k groups.
    /// </summary>
    public IReadOnlyList<(Priority Priority, int Count)> OpenCountByPriority() =>
        _tickets
            .Where(t => t.Status != TicketStatus.Closed)
            .GroupBy(t => t.Priority)
            .OrderByDescending(g => g.Key)
            .Select(g => (g.Key, g.Count()))
            .ToList();

    /// <summary>Async walkthrough operation: writes the summary, stopping promptly when cancelled.</summary>
    public async Task WriteSummaryAsync(TextWriter writer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(writer);

        foreach (var (priority, count) in OpenCountByPriority())
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync($"{priority}: {count}".AsMemory(), cancellationToken);
        }
    }
}
