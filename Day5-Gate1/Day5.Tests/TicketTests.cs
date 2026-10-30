using Day5.Core;

namespace Day5.Tests;

public class TicketTests
{
    private static readonly DateTime Now = new(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc);

    // ---- Normal inputs ----

    [Fact]
    public void Create_ValidInput_TrimsTitleAndStartsOpen()
    {
        // Arrange
        var service = new TicketService();

        // Act
        Ticket ticket = service.Create("  Login fails  ", Priority.High, Now);

        // Assert
        Assert.Equal("Login fails", ticket.Title);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(1, ticket.Id);
    }

    [Fact]
    public void Assign_OpenTicket_SetsAssigneeAndMovesToInProgress()
    {
        // Arrange
        var ticket = new Ticket(1, "t", Priority.Low, Now);

        // Act
        ticket.Assign("Alice");

        // Assert
        Assert.Equal("Alice", ticket.Assignee);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Fact]
    public void OpenCountByPriority_MixedTickets_GroupsMostUrgentFirstAndSkipsClosed()
    {
        // Arrange
        var service = new TicketService();
        service.Create("a", Priority.Low, Now);
        service.Create("b", Priority.Critical, Now);
        service.Create("c", Priority.Low, Now);
        service.Create("d", Priority.High, Now);
        service.Get(4).Close(Now);

        // Act
        var result = service.OpenCountByPriority();

        // Assert
        Assert.Equal(new[] { (Priority.Critical, 1), (Priority.Low, 2) }, result);
    }

    [Fact]
    public async Task WriteSummaryAsync_WritesOneLinePerPriority()
    {
        // Arrange
        var service = new TicketService();
        service.Create("a", Priority.Low, Now);
        service.Create("b", Priority.Critical, Now);
        using var writer = new StringWriter();

        // Act
        await service.WriteSummaryAsync(writer);

        // Assert
        Assert.Equal($"Critical: 1{Environment.NewLine}Low: 1{Environment.NewLine}", writer.ToString());
    }

    // ---- Boundary inputs ----

    [Theory]
    [InlineData(Priority.Critical, 4)]
    [InlineData(Priority.High, 24)]
    [InlineData(Priority.Medium, 72)]
    [InlineData(Priority.Low, 168)]
    public void IsOverdue_ExactlyAtSlaWindow_IsFalse(Priority priority, int slaHours)
    {
        // Arrange
        var ticket = new Ticket(1, "t", priority, Now.AddHours(-slaHours));

        // Act
        bool overdue = ticket.IsOverdue(Now);

        // Assert
        Assert.False(overdue);
    }

    [Fact]
    public void IsOverdue_OneMinutePastSla_IsTrue()
    {
        // Arrange
        var ticket = new Ticket(1, "t", Priority.High, Now.AddHours(-24).AddMinutes(-1));

        // Act & Assert
        Assert.True(ticket.IsOverdue(Now));
    }

    [Fact]
    public void Close_AtCreationTime_IsAllowed()
    {
        // Arrange
        var ticket = new Ticket(1, "t", Priority.Low, Now);

        // Act
        ticket.Close(Now);

        // Assert
        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }

    [Fact]
    public void OpenCountByPriority_NoTickets_ReturnsEmpty()
    {
        Assert.Empty(new TicketService().OpenCountByPriority());
    }

    // ---- Invalid inputs ----

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_BlankTitle_Throws(string? title)
    {
        // Arrange
        var service = new TicketService();

        // Act
        Action act = () => service.Create(title!, Priority.Low, Now);

        // Assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void Create_UndefinedPriority_Throws()
    {
        // Arrange
        var service = new TicketService();

        // Act
        Action act = () => service.Create("t", (Priority)99, Now);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Assign_BlankAssignee_Throws()
    {
        // Arrange
        var ticket = new Ticket(1, "t", Priority.Low, Now);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.Assign(" "));
    }

    [Fact]
    public void Close_BeforeCreation_Throws()
    {
        // Arrange
        var ticket = new Ticket(1, "t", Priority.Low, Now);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.Close(Now.AddMinutes(-1)));
    }

    [Fact]
    public void Assign_ClosedTicket_Throws()
    {
        // Arrange
        var ticket = new Ticket(1, "t", Priority.Low, Now);
        ticket.Close(Now);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ticket.Assign("Alice"));
    }

    [Fact]
    public void Get_UnknownId_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => new TicketService().Get(42));
    }

    [Fact]
    public async Task WriteSummaryAsync_CancelledToken_Throws()
    {
        // Arrange
        var service = new TicketService();
        service.Create("a", Priority.Low, Now);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = () => service.WriteSummaryAsync(new StringWriter(), cts.Token);

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(act);
    }
}
