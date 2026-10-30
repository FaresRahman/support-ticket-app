namespace Day4.Core;

public static class TicketExporter
{
    public const string Header = "Id,Title,Priority,Status";

    /// <summary>
    /// Writes tickets as CSV. Honors the cancellation token before every row, so a long export can be stopped.
    /// Throws OperationCanceledException when cancelled. Returns the number of rows written.
    /// </summary>
    public static async Task<int> ExportCsvAsync(IEnumerable<Ticket> tickets, TextWriter writer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tickets);
        ArgumentNullException.ThrowIfNull(writer);

        await writer.WriteLineAsync(Header.AsMemory(), cancellationToken);

        int rows = 0;
        foreach (Ticket ticket in tickets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string title = ticket.Title.Contains(',') ? $"\"{ticket.Title}\"" : ticket.Title;
            string line = $"{ticket.Id},{title},{ticket.Priority},{ticket.Status}";
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
            rows++;
        }

        return rows;
    }
}
