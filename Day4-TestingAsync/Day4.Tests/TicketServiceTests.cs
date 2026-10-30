using Day4.Core;

namespace Day4.Tests;

public class TicketServiceTests
{
    private static readonly DateTime Now = new(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Add_ValidTitle_AssignsSequentialIdsAndOpenStatus()
    {
        // Arrange
        var service = new TicketService();

        // Act
        Ticket first = service.Add("Login fails", Priority.High, Now);
        Ticket second = service.Add("Slow page", Priority.Low, Now);

        // Assert
        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
        Assert.Equal(TicketStatus.Open, first.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_BlankTitle_ThrowsArgumentException(string title)
    {
        // Arrange
        var service = new TicketService();

        // Act
        Action act = () => service.Add(title, Priority.Low, Now);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Close_ExistingTicket_SetsStatusAndClosedTime()
    {
        // Arrange
        var service = new TicketService();
        Ticket ticket = service.Add("Login fails", Priority.High, Now);

        // Act
        service.Close(ticket.Id, Now.AddHours(2));

        // Assert
        Assert.Equal(TicketStatus.Closed, ticket.Status);
        Assert.Equal(Now.AddHours(2), ticket.ClosedUtc);
    }

    [Fact]
    public void Close_UnknownId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new TicketService();

        // Act
        Action act = () => service.Close(99, Now);

        // Assert
        Assert.Throws<KeyNotFoundException>(act);
    }

    [Fact]
    public void FilterByMinPriority_High_ReturnsHighAndCriticalOnly()
    {
        // Arrange
        var service = new TicketService();
        service.Add("a", Priority.Low, Now);
        service.Add("b", Priority.High, Now);
        service.Add("c", Priority.Critical, Now);

        // Act
        IReadOnlyList<Ticket> result = service.FilterByMinPriority(Priority.High);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("b", result[0].Title);
        Assert.Equal("c", result[1].Title);
    }

    [Fact]
    public void GetOverdue_TicketExactlyAtSlaWindow_IsNotOverdue()
    {
        // Arrange: High priority SLA is 24h
        var service = new TicketService();
        service.Add("boundary", Priority.High, Now.AddHours(-24));

        // Act
        IReadOnlyList<Ticket> overdue = service.GetOverdue(Now);

        // Assert
        Assert.Empty(overdue);
    }

    [Fact]
    public void GetOverdue_OpenTicketPastSla_IsReturnedAndClosedIsIgnored()
    {
        // Arrange
        var service = new TicketService();
        Ticket late = service.Add("late", Priority.High, Now.AddHours(-25));
        Ticket closed = service.Add("closed", Priority.High, Now.AddHours(-25));
        service.Close(closed.Id, Now);

        // Act
        IReadOnlyList<Ticket> overdue = service.GetOverdue(Now);

        // Assert
        Assert.Equal(new[] { late.Id }, overdue.Select(t => t.Id));
    }
}
