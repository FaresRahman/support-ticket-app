namespace Day3.Linq;

/// <summary>Pure, unit-testable query methods written as extension methods over IEnumerable&lt;Ticket&gt;.</summary>
public static class TicketQueries
{
    public static IEnumerable<Ticket> WithStatus(this IEnumerable<Ticket> tickets, TicketStatus status) =>
        tickets.Where(t => t.Status == status);

    public static IEnumerable<Ticket> AtLeast(this IEnumerable<Ticket> tickets, Priority minimum) =>
        tickets.Where(t => t.Priority >= minimum);

    public static IEnumerable<Ticket> Unassigned(this IEnumerable<Ticket> tickets) =>
        tickets.Where(t => t.Assignee is null);

    public static IEnumerable<Ticket> TitleContains(this IEnumerable<Ticket> tickets, string text) =>
        tickets.Where(t => t.Title.Contains(text, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<Ticket> Overdue(this IEnumerable<Ticket> tickets, DateTime nowUtc) =>
        tickets.Where(t => t.Status != TicketStatus.Closed && nowUtc - t.CreatedUtc > Sla.For(t.Priority));

    public static IEnumerable<Ticket> ByPriorityThenAge(this IEnumerable<Ticket> tickets) =>
        tickets.OrderByDescending(t => t.Priority).ThenBy(t => t.CreatedUtc);

    public static IEnumerable<Ticket> Oldest(this IEnumerable<Ticket> tickets, int count) =>
        tickets.OrderBy(t => t.CreatedUtc).Take(count);

    public static Dictionary<string, int> CountByAssignee(this IEnumerable<Ticket> tickets) =>
        tickets.Where(t => t.Assignee is not null)
               .GroupBy(t => t.Assignee!)
               .ToDictionary(g => g.Key, g => g.Count());

    public static IEnumerable<IGrouping<string, Ticket>> GroupByCategory(this IEnumerable<Ticket> tickets) =>
        tickets.GroupBy(t => t.Category);

    public static Dictionary<int, Ticket> IndexById(this IEnumerable<Ticket> tickets) =>
        tickets.ToDictionary(t => t.Id);

    public static double? AverageResolutionHours(this IEnumerable<Ticket> tickets)
    {
        var hours = tickets
            .Where(t => t.ClosedUtc is not null)
            .Select(t => (t.ClosedUtc!.Value - t.CreatedUtc).TotalHours)
            .ToList();
        return hours.Count == 0 ? null : hours.Average();
    }

    /// <summary>Pattern matching: switch expression with property patterns.</summary>
    public static string Describe(Ticket t) => t switch
    {
        { Priority: Priority.Critical, Assignee: null } => $"URGENT, UNASSIGNED: {t.Title}",
        { Priority: Priority.Critical } => $"CRITICAL: {t.Title} (assigned to {t.Assignee})",
        { Status: TicketStatus.Closed } => $"Closed: {t.Title}",
        { Assignee: null } => $"Needs owner: {t.Title}",
        _ => $"{t.Title} [{t.Status}]"
    };
}
