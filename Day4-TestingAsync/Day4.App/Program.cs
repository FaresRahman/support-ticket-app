using Day4.Core;

var service = new TicketService();
DateTime now = DateTime.UtcNow;
service.Add("Login fails", Priority.High, now.AddHours(-30));
service.Add("Slow dashboard, intermittent", Priority.Medium, now.AddHours(-10));
service.Add("Payment gateway timeout", Priority.Critical, now.AddHours(-5));

// Ctrl+C cancels the export; a 5 second timeout is the safety net.
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    int rows = await TicketExporter.ExportCsvAsync(service.All, Console.Out, cts.Token);
    Console.WriteLine($"Exported {rows} tickets.");
    Console.WriteLine($"Overdue: {service.GetOverdue(now).Count}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Export cancelled.");
}
