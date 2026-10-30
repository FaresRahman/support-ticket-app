namespace SupportTickets.Domain;

/// <summary>Application logic. Depends on the ITicketRepository abstraction, injected through the constructor.</summary>
public class TicketService
{
    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Ticket Open(string title, string description, Priority priority, Customer requester, DateTime nowUtc)
    {
        var ticket = new Ticket(_repository.NextId(), title, description, priority, requester, nowUtc);
        _repository.Add(ticket);
        return ticket;
    }

    public void Assign(int ticketId, Agent agent) => Get(ticketId).Assign(agent);

    public void Close(int ticketId, DateTime nowUtc) => Get(ticketId).Close(nowUtc);

    public IReadOnlyList<Ticket> Overdue(DateTime nowUtc) =>
        _repository.GetAll().Where(t => t.IsOverdue(nowUtc)).ToList();

    private Ticket Get(int id) =>
        _repository.GetById(id) ?? throw new KeyNotFoundException($"Ticket {id} not found.");
}
