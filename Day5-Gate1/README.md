# Day 5 – Gate 1 mini-build: Support Ticket Core

Solution `Day5.sln`: `Day5.Core` (class library), `Day5.App` (console demo), `Day5.Tests` (xUnit).

```powershell
cd Day5-Gate1
dotnet restore
dotnet build
dotnet test
dotnet run --project Day5.App
```

Verified: the build succeeds with 0 warnings and all 21 tests pass. With the `>=` defect injected, 5 tests fail (the regression test and the four boundary cases); with the fix they pass.

## What it does

Create tickets, assign them, close them, find those past their SLA, and summarise open tickets by priority (also as an async, cancellable write).

## Tests (`Day5.Tests`)

| Kind | Examples |
|---|---|
| Normal | create trims title; assign moves to InProgress; grouping excludes closed; summary output |
| Boundary | exactly-at-SLA is not overdue (all four priorities); one minute past is overdue; close at creation time; empty list |
| Invalid | blank/null title; undefined priority; blank assignee; close before creation; assign to closed ticket; unknown id; cancelled token |
| Regression | `RegressionTests.GetOverdue_TicketExactlyAtDeadline_...` |

## Injected defect and regression test

The defect to inject is in `Ticket.IsOverdue` (`Day5.Core/Ticket.cs`): changing `>` to `>=`, so a ticket exactly at its SLA deadline is wrongly overdue.
To demonstrate: make that change, run `dotnet test` and see `RegressionTests` plus the four `IsOverdue_ExactlyAtSlaWindow_IsFalse` cases fail, then restore `>` and see them pass.
The submitted code has the fix applied. If the mentor injects a different defect, add a regression test for it in `RegressionTests.cs` the same way: reproduce it with a failing test first, then fix.

## Walkthrough guide

- **One class:** `Ticket` – encapsulated state (`private set`), transitions only through `Assign`/`Close`, invariants enforced (no close before creation, no changes after closing). Single Responsibility: the ticket owns its rules; `TicketService` owns the collection.
- **One LINQ query:** `TicketService.OpenCountByPriority` – `Where` (drop closed) -> `GroupBy` priority -> `OrderByDescending` key -> `Select` (priority, count). Deferred until `ToList()`. O(n) to group plus sorting the k <= 4 groups.
- **One async operation:** `TicketService.WriteSummaryAsync` – takes a `CancellationToken`, checks it before each line, passes it to `WriteLineAsync`; cancellation surfaces as `OperationCanceledException`. Pitfalls to mention: don't use `.Result`/`.Wait()` (deadlocks), an `async void` handler can't be awaited, and a cancelled token must be observed or it does nothing.

## Interview prep: SOLID principle in this code

Single Responsibility: `Ticket` (rules), `TicketService` (collection and queries), `Sla` (policy lookup), `Program` (presentation) each change for one reason.
