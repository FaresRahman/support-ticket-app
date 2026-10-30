namespace SupportTickets.Domain;

/// <summary>Abstraction the rest of the app depends on; storage details live behind it.</summary>
public interface ITicketRepository
{
    void Add(Ticket ticket);
    Ticket? GetById(int id);
    IReadOnlyList<Ticket> GetAll();
    int NextId();
}
