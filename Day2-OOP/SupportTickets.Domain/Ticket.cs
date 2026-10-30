namespace SupportTickets.Domain;

/// <summary>
/// Entity with encapsulated state: fields change only through methods that enforce the rules.
/// Ticket *has* a Customer, an optional Agent and Comments (composition/association), it does not inherit from them.
/// </summary>
public class Ticket
{
    private readonly List<Comment> _comments = new();

    public Ticket(int id, string title, string description, Priority priority, Customer requester, DateTime createdUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        Id = id;
        Title = title.Trim();
        Description = description ?? string.Empty;
        Priority = priority;
        Requester = requester ?? throw new ArgumentNullException(nameof(requester));
        CreatedUtc = createdUtc;
        Status = TicketStatus.Open;
    }

    public int Id { get; }
    public string Title { get; }
    public string Description { get; }
    public Priority Priority { get; }
    public Customer Requester { get; }
    public DateTime CreatedUtc { get; }

    public TicketStatus Status { get; private set; }
    public Agent? AssignedTo { get; private set; }
    public DateTime? ClosedUtc { get; private set; }

    public IReadOnlyList<Comment> Comments => _comments;

    public void Assign(Agent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);
        EnsureNotClosed();
        AssignedTo = agent;
        Status = TicketStatus.InProgress;
    }

    public void AddComment(Person author, string text, DateTime postedUtc)
    {
        ArgumentNullException.ThrowIfNull(author);
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Comment text is required.", nameof(text));
        }

        _comments.Add(new Comment(author.Name, text.Trim(), postedUtc));
    }

    public void Close(DateTime closedUtc)
    {
        EnsureNotClosed();
        Status = TicketStatus.Closed;
        ClosedUtc = closedUtc;
    }

    public bool IsOverdue(DateTime nowUtc) =>
        Status != TicketStatus.Closed && nowUtc - CreatedUtc > SlaPolicy.For(Priority).ResponseWindow;

    private void EnsureNotClosed()
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException($"Ticket {Id} is already closed.");
        }
    }
}
