# Day 3 – LINQ queries: input, output, complexity

**Input for all queries:** the 8 tickets in `Day3.Linq/Models.cs` (`SampleData.Tickets()`), `now = 2026-10-13 00:00 UTC`.
n = number of tickets, k = number of groups, m = title length.

| # | Query (method) | Output | Complexity |
|---|---|---|---|
| Q1 | `WithStatus(Open)` – `Where` | `[1, 4, 7, 8]` | O(n) time, O(1) extra (streamed) |
| Q2 | `AtLeast(High)` – `Where` on enum order | `[1, 3, 6, 8]` | O(n), O(1) |
| Q3 | `Unassigned()` – `Where` + `is null` | `[4, 7]` | O(n), O(1) |
| Q4 | `CountByAssignee()` – `GroupBy` + `ToDictionary` | `Alice=2, Bob=2, Carol=2` | O(n) time, O(n) space |
| Q5 | `GroupByCategory()` – `GroupBy` | `Auth=2, Content=1, Billing=2, Reports=2, UI=1` | O(n) time, O(n) space |
| Q6 | `ByPriorityThenAge()` – `OrderByDescending` + `ThenBy` | `[3, 1, 6, 8, 4, 5, 2, 7]` | O(n log n) time, O(n) space |
| Q7 | `TitleContains("pass")` – `Where` + `Contains` | `[6]` | O(n·m) time, O(1) |
| Q8 | `IndexById()` then `byId[4]` – `ToDictionary` | `Export to CSV broken` | Build O(n) time/O(n) space; each lookup O(1) average |
| Q9 | `AverageResolutionHours()` – `Where` + `Select` + `Average` | `30.0` (closed: 24h and 36h) | O(n) time, O(c) space for c closed tickets |
| Q10 | `Overdue(now)` – `Where` with SLA rule | `[1, 3, 4, 5]` (ticket 8 is exactly 24h old on a 24h SLA, so **not** overdue) | O(n), O(1) |
| Q11 | `WithStatus(Open).Oldest(2)` – `Where` + `OrderBy` + `Take` | `[1, 4]` | O(n log n) time (full sort), O(n) space |
| PM | `Describe(ticket 3)` – switch expression, property patterns | `CRITICAL: Payment gateway timeout (assigned to Alice)` | O(1) |
| DE | `Where` query created, then an item added, then enumerated | `[1, 4, 7, 8, 9]` | Deferred: nothing runs until enumeration; each enumeration is O(n) |

Expected console output of `dotnet run`:

```
Q1  Open tickets:            [1, 4, 7, 8]
Q2  High or above:           [1, 3, 6, 8]
Q3  Unassigned:              [4, 7]
Q4  Count by assignee:       Alice=2, Bob=2, Carol=2
Q5  Count by category:       Auth=2, Content=1, Billing=2, Reports=2, UI=1
Q6  Priority desc, age asc:  [3, 1, 6, 8, 4, 5, 2, 7]
Q7  Title contains 'pass':   [6]
Q8  Ticket #4 title:         Export to CSV broken
Q9  Avg resolution hours:    30.0
Q10 Overdue at 2026-10-13:  [1, 3, 4, 5]
Q11 Two oldest open:         [1, 4]
PM  Describe #3:             CRITICAL: Payment gateway timeout (assigned to Alice)
DE  Open after later Add:    [1, 4, 7, 8, 9]
```

## Interview prep notes

- **IEnumerable vs IQueryable:** `IEnumerable<T>` runs LINQ in memory with delegates (used here). `IQueryable<T>` builds an expression tree that a provider (e.g. EF Core) translates to SQL, so filtering happens in the database.
- **Deferred execution:** `Where`, `Select`, `OrderBy` only describe the query; it runs on enumeration (`foreach`, `ToList`, `Count`...). `ToList`/`ToDictionary` execute immediately. See the DE row.
- **List vs Dictionary:** `List<T>` lookup by value is O(n); `Dictionary<K,V>` lookup by key is O(1) average, at the cost of memory and no ordering guarantee.
