# Value and reference types used

| Type | Kind | Why |
|---|---|---|
| `int`, `double`, `bool` | Value | Counters, averages, `IsOpen`. Stored inline, copied on assignment. |
| `Priority` (enum) | Value | A small fixed set of named ints; cheap to copy and compare. |
| `TicketStats` (readonly struct) | Value | A small immutable result bundle. Passing it around copies it, so no caller can change another caller's numbers. (Its `int[]` field is still a reference, so the array is shared – a good illustration that a struct can hold references.) |
| `Ticket` (class) | Reference | Has identity and changing state (`IsOpen`, `AssignedTo`). `Tickets` holds references, so `Find` returns the *same* objects, not copies. |
| `string` | Reference (immutable) | Behaves like a value because it cannot change; `Trim()` etc. return new strings. |
| `string?` (`AssignedTo`) | Nullable reference | Compiler warns if it is dereferenced without a null check. |
| `int[]`, `List<Ticket>` | Reference | Arrays and lists are objects on the heap. |

## Key ideas

- **Value vs reference:** assigning a value type copies the data; assigning a reference type copies the pointer, so both variables see the same object.
- **string vs StringBuilder:** strings are immutable, so repeated `+=` allocates repeatedly. `FormatList` and `FormatSummary` use `StringBuilder`.
- **Nullable reference types:** `<Nullable>enable</Nullable>` makes `string` non-null by default and `string?` explicitly nullable. This is a compile-time check only; it does not change runtime behaviour.
- **Nullable value types:** `int?` is `Nullable<int>`, a different mechanism (not used here, see Day 3 `DateTime? ClosedUtc`).
