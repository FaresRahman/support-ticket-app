# Day 2 – Object-oriented C#: Support Ticket domain model

Class library `SupportTickets.Domain` (no entry point). Build with:

```powershell
cd Day2-OOP\SupportTickets.Domain
dotnet build
```

| Requirement | Where |
|---|---|
| Domain model with at least 3 classes | `Person`, `Customer`, `Agent`, `Ticket`, `Comment` (record), `SlaPolicy` (struct), `TicketService` |
| Interface-based design with one implementation | `ITicketRepository` -> `InMemoryTicketRepository` |
| Class relationship diagram | [ClassDiagram.md](ClassDiagram.md) (Mermaid) |
| Short SOLID explanation | [SOLID.md](SOLID.md) |

Interview prep covered in `SOLID.md`: interface vs abstract class, composition vs inheritance, SOLID with examples.
