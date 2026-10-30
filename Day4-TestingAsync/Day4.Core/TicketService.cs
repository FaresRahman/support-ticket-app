namespace Day4.Core;

public class TicketService
{
    private readonly List<Ticket> _tickets = new();
    private int _nextId = 1;

    public IReadOnlyList<Ticket> All => _tickets;

    public Ticket Add(string title, Priority priority, DateTime createdUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        var ticket = new Ticket(_nextId++, title.Trim(), priority, createdUtc);
        _tickets.Add(ticket);
        return ticket;
    }

    public Ticket Close(int id, DateTime closedUtc)
    {
        Ticket ticket = _tickets.FirstOrDefault(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Ticket {id} not found.");
        ticket.Status = TicketStatus.Closed;
        ticket.ClosedUtc = closedUtc;
        return ticket;
    }

    public IReadOnlyList<Ticket> FilterByMinPriority(Priority minimum) =>
        _tickets.Where(t => t.Priority >= minimum).ToList();

    /// <summary>Open tickets older than their SLA window. A ticket exactly at the window is not yet overdue.</summary>
    public IReadOnlyList<Ticket> GetOverdue(DateTime nowUtc) =>
        _tickets.Where(t => t.Status == TicketStatus.Open && nowUtc - t.CreatedUtc > Sla.For(t.Priority)).ToList();
}
