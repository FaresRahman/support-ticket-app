# Day 1 – C# Fundamentals: Support Ticket Console

A console app that records support tickets in memory and lets you list, search and summarise them.
It uses methods, `List<T>`, arrays, enums, a struct, string handling and exception handling.

## Run

Requires a .NET SDK (project targets `net8.0`; confirm the team's version and change `TargetFramework` if needed).

```powershell
cd Day1-CSharpFundamentals
dotnet run
```

Commands (one per line, `quit` to exit):

| Command | Meaning |
|---|---|
| `add <Priority>\|<Title>` | Add a ticket. Priority: Low, Medium, High, Critical |
| `list` | Show all tickets |
| `find <text>` | Case-insensitive title search |
| `summary` | Totals, count per priority, average title length |

## Sample inputs and outputs

Each sample can be piped in, e.g. `"add High|Login page returns 500`nlist" | dotnet run`.

### Sample 1 – add, list, find

Input
```
add High|Login page returns 500
add Low|Update footer text
list
find footer
```
Output
```
Added #1 [High] Login page returns 500
Added #2 [Low] Update footer text
#1 [High] Login page returns 500 (Open)
#2 [Low] Update footer text (Open)
#2 [Low] Update footer text (Open)
```

### Sample 2 – invalid input is handled, not crashed

Input
```
add Urgent|Something
add High|
list
```
Output
```
Error: 'Urgent' is not a valid priority (Low, Medium, High, Critical).
Error: title is required (usage: add <Priority>|<Title>).
No tickets.
```

### Sample 3 – summary

Input
```
add High|Login page returns 500
add Critical|Database down
add Low|Update footer text
summary
```
Output
```
Added #1 [High] Login page returns 500
Added #2 [Critical] Database down
Added #3 [Low] Update footer text
Total: 3
Open: 3
By priority: Low=1, Medium=0, High=1, Critical=1
Average title length: 17.7
```

## Files

- `Program.cs` – command loop, parsing, search and statistics methods
- `Ticket.cs` – `Priority` enum, `Ticket` class, `TicketStats` struct
- `ValueAndReferenceTypes.md` – short note on value vs reference types used
