using Day4.Core;

namespace Day4.Tests;

public class TicketExporterTests
{
    private static readonly DateTime Now = new(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ExportCsvAsync_WritesHeaderAndOneLinePerTicket()
    {
        // Arrange
        var service = new TicketService();
        service.Add("Login fails", Priority.High, Now);
        service.Add("Slow, dashboard", Priority.Low, Now);
        using var writer = new StringWriter();

        // Act
        int rows = await TicketExporter.ExportCsvAsync(service.All, writer);

        // Assert
        string[] lines = writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, rows);
        Assert.Equal(TicketExporter.Header, lines[0]);
        Assert.Equal("1,Login fails,High,Open", lines[1]);
        Assert.Equal("2,\"Slow, dashboard\",Low,Open", lines[2]);
    }

    [Fact]
    public async Task ExportCsvAsync_AlreadyCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var service = new TicketService();
        service.Add("Login fails", Priority.High, Now);
        using var writer = new StringWriter();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = () => TicketExporter.ExportCsvAsync(service.All, writer, cts.Token);

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(act);
    }
}
