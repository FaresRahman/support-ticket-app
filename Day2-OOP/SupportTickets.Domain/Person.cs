namespace SupportTickets.Domain;

/// <summary>Abstract base: shared state and validation, with a role that each subclass defines.</summary>
public abstract class Person
{
    protected Person(int id, string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("A valid email is required.", nameof(email));
        }

        Id = id;
        Name = name.Trim();
        Email = email.Trim();
    }

    public int Id { get; }
    public string Name { get; }
    public string Email { get; }

    public abstract string Role { get; }

    public override string ToString() => $"{Name} ({Role})";
}

public sealed class Customer : Person
{
    public Customer(int id, string name, string email, string? company = null)
        : base(id, name, email)
    {
        Company = company;
    }

    public string? Company { get; }

    public override string Role => "Customer";
}

public sealed class Agent : Person
{
    public Agent(int id, string name, string email, string team)
        : base(id, name, email)
    {
        if (string.IsNullOrWhiteSpace(team))
        {
            throw new ArgumentException("Team is required.", nameof(team));
        }

        Team = team;
    }

    public string Team { get; }

    public override string Role => "Agent";
}
