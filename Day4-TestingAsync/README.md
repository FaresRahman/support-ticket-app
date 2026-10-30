# Day 4 – Unit testing, async C# and .NET tooling

Solution `Day4.sln` with three projects:

| Project | Type | Purpose |
|---|---|---|
| `Day4.Core` | class library | `Ticket`, `TicketService`, async `TicketExporter` |
| `Day4.App` | console app | Runs the async export with a timeout and Ctrl+C cancellation |
| `Day4.Tests` | xUnit test project | 9 test methods / 10 cases (Arrange-Act-Assert) |

```powershell
cd Day4-TestingAsync
dotnet restore
dotnet build
dotnet test
dotnet run --project Day4.App
```

Output of `dotnet build` and `dotnet test` (10 passed) is saved in [build-test-log.txt](build-test-log.txt).

## Async with cancellation

`TicketExporter.ExportCsvAsync(tickets, writer, cancellationToken)` checks `ThrowIfCancellationRequested()` before every row and passes the token to `WriteLineAsync`. Cancelling throws `OperationCanceledException`, which the caller handles (`Day4.App/Program.cs`). Tested with a pre-cancelled token, so the test is deterministic (no sleeps).

## Testing choices

- Time is passed in (`DateTime now`) instead of calling `DateTime.UtcNow` inside the code, so tests are deterministic.
- Names follow `Method_Scenario_ExpectedResult`.
- Edge cases: blank titles (`[Theory]`), unknown id, SLA boundary (exactly 24h is *not* overdue), closed tickets ignored, cancelled token.

## Interview prep

- **Good unit test:** fast, isolated, deterministic, one reason to fail, clear name.
- **Arrange-Act-Assert:** set up, execute one action, verify the outcome.
- **Task vs Thread:** a `Thread` is an OS execution unit; a `Task` is a promise of a result scheduled on the thread pool. `await` frees the thread while waiting on I/O.
- **restore / build / test:** `restore` downloads NuGet packages, `build` compiles, `test` builds (unless `--no-build`) then runs the test runner.
