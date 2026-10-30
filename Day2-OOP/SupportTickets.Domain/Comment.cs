namespace SupportTickets.Domain;

/// <summary>Record: immutable data carrier with value-based equality.</summary>
public sealed record Comment(string Author, string Text, DateTime PostedUtc);
