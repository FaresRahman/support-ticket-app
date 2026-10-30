# SOLID in this design

| Principle | Where it shows up |
|---|---|
| **S – Single Responsibility** | `Ticket` guards its own rules (assign/close); `TicketService` orchestrates use cases; `InMemoryTicketRepository` only stores. Each has one reason to change. |
| **O – Open/Closed** | A new storage backend (e.g. SQL) is a new class implementing `ITicketRepository`; `TicketService` is not edited. |
| **L – Liskov Substitution** | Anywhere a `Person` is accepted (e.g. `Ticket.AddComment`) a `Customer` or `Agent` works with no special cases. Any `ITicketRepository` can replace the in-memory one. |
| **I – Interface Segregation** | `ITicketRepository` has four small members, all needed by its consumer. Splitting into read/write interfaces would be the next step if consumers needed only part. |
| **D – Dependency Inversion** | `TicketService` depends on the `ITicketRepository` abstraction, supplied via its constructor, not on `new InMemoryTicketRepository()`. This also lets tests pass a fake. |

## Composition over inheritance

`Ticket` does not inherit from `Customer`/`Agent`; it holds references to them. The only inheritance is `Person -> Customer/Agent`, where "is-a" is true and shared state/validation is reused.
`SlaPolicy` is composed in (looked up by priority) rather than creating `CriticalTicket`, `LowTicket` subclasses.

## Interface vs abstract class (as used here)

- `ITicketRepository` is a **contract** with no state: unrelated classes can implement it, and a class can implement many.
- `Person` is an **abstract class** because `Customer` and `Agent` share state (`Id`, `Name`, `Email`) and constructor validation, and must supply `Role`.

## Other Day 2 topics used

- **Encapsulation / access modifiers:** `private set` on `Status`, `AssignedTo`, `ClosedUtc`; private `_comments` exposed as `IReadOnlyList`; `protected` constructor on `Person`.
- **Polymorphism:** `Person.Role` is overridden; `person.ToString()` prints the correct role without type checks.
- **Record vs struct:** `Comment` is a reference-type record (equality by value); `SlaPolicy` is a `readonly record struct`.
