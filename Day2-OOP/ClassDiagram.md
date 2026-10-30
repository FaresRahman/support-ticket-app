# Class relationship diagram

Renders on GitHub, VS Code (Markdown Preview Mermaid) and any Mermaid viewer.

```mermaid
classDiagram
    class Person {
        <<abstract>>
        +int Id
        +string Name
        +string Email
        +string Role*
    }
    class Customer {
        +string? Company
        +string Role
    }
    class Agent {
        +string Team
        +string Role
    }
    class Ticket {
        +int Id
        +string Title
        +Priority Priority
        +TicketStatus Status
        +DateTime CreatedUtc
        +DateTime? ClosedUtc
        +Assign(Agent)
        +AddComment(Person, string, DateTime)
        +Close(DateTime)
        +IsOverdue(DateTime) bool
    }
    class Comment {
        <<record>>
        +string Author
        +string Text
        +DateTime PostedUtc
    }
    class SlaPolicy {
        <<readonly record struct>>
        +TimeSpan ResponseWindow
        +For(Priority)$ SlaPolicy
    }
    class ITicketRepository {
        <<interface>>
        +Add(Ticket)
        +GetById(int) Ticket?
        +GetAll() IReadOnlyList~Ticket~
        +NextId() int
    }
    class InMemoryTicketRepository
    class TicketService {
        +Open(...) Ticket
        +Assign(int, Agent)
        +Close(int, DateTime)
        +Overdue(DateTime)
    }

    Person <|-- Customer
    Person <|-- Agent
    Ticket "1" --> "1" Customer : requester
    Ticket "1" --> "0..1" Agent : assignedTo
    Ticket "1" *-- "0..*" Comment : comments
    Ticket ..> SlaPolicy : uses
    ITicketRepository <|.. InMemoryTicketRepository
    TicketService --> ITicketRepository : depends on
    ITicketRepository ..> Ticket : stores
```

## Reading the diagram

- **Inheritance (`<|--`)**: `Customer` and `Agent` are kinds of `Person`.
- **Implementation (`<|..`)**: `InMemoryTicketRepository` implements `ITicketRepository`.
- **Composition (`*--`)**: a `Ticket` owns its `Comment`s; they have no life outside it.
- **Association (`-->`)**: a `Ticket` refers to a `Customer` and optionally an `Agent`, which exist independently.
