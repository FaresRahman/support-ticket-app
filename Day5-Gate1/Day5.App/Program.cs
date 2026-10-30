using Day5.Core;

var service = new TicketService();
DateTime now = DateTime.UtcNow;

service.Create("Login fails", Priority.High, now.AddHours(-30));
service.Create("Payment gateway timeout", Priority.Critical, now.AddHours(-2));
service.Create("Dark mode request", Priority.Low, now.AddDays(-1));
service.Get(2).Assign("Alice");
service.Create("Old issue", Priority.Medium, now.AddDays(-10));
service.Get(4).Close(now);

Console.WriteLine("Open tickets by priority:");
await service.WriteSummaryAsync(Console.Out);

Console.WriteLine("Overdue:");
foreach (Ticket t in service.GetOverdue(now))
{
    Console.WriteLine($"  #{t.Id} {t.Title} ({t.Priority})");
}
