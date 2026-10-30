namespace SupportTickets.Domain;

public class InMemoryTicketRepository : ITicketRepository
{
    private readonly Dictionary<int, Ticket> _tickets = new();
    private int _lastId;

    public void Add(Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        if (!_tickets.TryAdd(ticket.Id, ticket))
        {
            throw new InvalidOperationException($"Ticket {ticket.Id} already exists.");
        }
    }

    public Ticket? GetById(int id) => _tickets.GetValueOrDefault(id);

    public IReadOnlyList<Ticket> GetAll() => _tickets.Values.OrderBy(t => t.Id).ToList();

    public int NextId() => ++_lastId;
}
