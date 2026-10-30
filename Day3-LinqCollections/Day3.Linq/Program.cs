using Day3.Linq;

// LINQ exercise file: 11 queries over the sample tickets. See QUERIES.md for input/output/complexity.
List<Ticket> tickets = SampleData.Tickets();
DateTime now = SampleData.Now;

static string Ids(IEnumerable<Ticket> source) => "[" + string.Join(", ", source.Select(t => t.Id)) + "]";

// Q1 Where: open tickets
Console.WriteLine($"Q1  Open tickets:            {Ids(tickets.WithStatus(TicketStatus.Open))}");

// Q2 Where + comparison: High or Critical
Console.WriteLine($"Q2  High or above:           {Ids(tickets.AtLeast(Priority.High))}");

// Q3 Where + null check: unassigned
Console.WriteLine($"Q3  Unassigned:              {Ids(tickets.Unassigned())}");

// Q4 GroupBy + ToDictionary: tickets per assignee
Console.WriteLine($"Q4  Count by assignee:       {string.Join(", ", tickets.CountByAssignee().Select(kv => $"{kv.Key}={kv.Value}"))}");

// Q5 GroupBy: tickets per category
Console.WriteLine($"Q5  Count by category:       {string.Join(", ", tickets.GroupByCategory().Select(g => $"{g.Key}={g.Count()}"))}");

// Q6 OrderByDescending + ThenBy: most urgent, oldest first
Console.WriteLine($"Q6  Priority desc, age asc:  {Ids(tickets.ByPriorityThenAge())}");

// Q7 Search: case-insensitive title search
Console.WriteLine($"Q7  Title contains 'pass':   {Ids(tickets.TitleContains("pass"))}");

// Q8 Dictionary lookup: O(1) by id after an O(n) build
Dictionary<int, Ticket> byId = tickets.IndexById();
Console.WriteLine($"Q8  Ticket #4 title:         {byId[4].Title}");

// Q9 Average over closed tickets
Console.WriteLine($"Q9  Avg resolution hours:    {tickets.AverageResolutionHours():F1}");

// Q10 Where with SLA rule
Console.WriteLine($"Q10 Overdue at {now:yyyy-MM-dd}:  {Ids(tickets.Overdue(now))}");

// Q11 Where + OrderBy + Take: two oldest open tickets
Console.WriteLine($"Q11 Two oldest open:         {Ids(tickets.WithStatus(TicketStatus.Open).Oldest(2))}");

// Pattern matching
Console.WriteLine($"PM  Describe #3:             {TicketQueries.Describe(tickets[2])}");

// Deferred execution: the query runs when enumerated, so it sees the later Add.
var openQuery = tickets.WithStatus(TicketStatus.Open);
tickets.Add(new Ticket(9, "New outage", Priority.Critical, TicketStatus.Open, null, "Infra", now, null));
Console.WriteLine($"DE  Open after later Add:    {Ids(openQuery)}");
