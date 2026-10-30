using Day5.Core;

namespace Day5.Tests;

public class RegressionTests
{
    private static readonly DateTime Now = new(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Regression for the injected defect: IsOverdue used &gt;= instead of &gt;, so a ticket exactly at its
    /// SLA deadline was reported overdue. This test fails against the defective code and passes after the fix.
    /// </summary>
    [Fact]
    public void GetOverdue_TicketExactlyAtDeadline_IsNotListedButOneMinuteLaterIs()
    {
        // Arrange
        var service = new TicketService();
        Ticket atDeadline = service.Create("at deadline", Priority.High, Now.AddHours(-24));
        Ticket pastDeadline = service.Create("past deadline", Priority.High, Now.AddHours(-24).AddMinutes(-1));

        // Act
        IReadOnlyList<Ticket> overdue = service.GetOverdue(Now);

        // Assert
        Assert.DoesNotContain(atDeadline, overdue);
        Assert.Contains(pastDeadline, overdue);
    }
}
