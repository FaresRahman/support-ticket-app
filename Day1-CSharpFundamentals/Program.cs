using System.Globalization;
using System.Text;

namespace Day1.TicketConsole;

public static class Program
{
    private static readonly List<Ticket> Tickets = new();
    private static int _nextId = 1;

    public static void Main()
    {
        if (!Console.IsInputRedirected)
        {
            Console.WriteLine("Support Ticket Console. Commands: add <Priority>|<Title>, list, find <text>, summary, quit");
        }

        string? line;
        while ((line = Console.ReadLine()) is not null)
        {
            line = line.Trim('﻿', ' ', '\t');
            if (line.Length == 0)
            {
                continue;
            }

            if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            try
            {
                Console.WriteLine(Execute(line));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static string Execute(string line)
    {
        int space = line.IndexOf(' ');
        string command = space < 0 ? line : line[..space];
        string argument = space < 0 ? string.Empty : line[(space + 1)..].Trim();

        return command.ToLowerInvariant() switch
        {
            "add" => Add(argument),
            "list" => FormatList(Tickets),
            "find" => FormatList(Find(argument)),
            "summary" => FormatSummary(ComputeStats(Tickets)),
            _ => throw new ArgumentException($"unknown command '{command}'.")
        };
    }

    private static string Add(string argument)
    {
        string[] parts = argument.Split('|', 2);
        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
        {
            throw new ArgumentException("title is required (usage: add <Priority>|<Title>).");
        }

        string priorityText = parts[0].Trim();
        if (!Enum.TryParse(priorityText, ignoreCase: true, out Priority priority) || !Enum.IsDefined(priority))
        {
            throw new ArgumentException($"'{priorityText}' is not a valid priority (Low, Medium, High, Critical).");
        }

        var ticket = new Ticket(_nextId++, parts[1].Trim(), priority);
        Tickets.Add(ticket);
        return $"Added #{ticket.Id} [{ticket.Priority}] {ticket.Title}";
    }

    public static List<Ticket> Find(string text)
    {
        var matches = new List<Ticket>();
        foreach (Ticket ticket in Tickets)
        {
            if (ticket.Title.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                matches.Add(ticket);
            }
        }

        return matches;
    }

    public static TicketStats ComputeStats(IReadOnlyList<Ticket> tickets)
    {
        int[] counts = new int[Enum.GetValues<Priority>().Length];
        int open = 0;
        int titleLength = 0;

        foreach (Ticket ticket in tickets)
        {
            counts[(int)ticket.Priority]++;
            titleLength += ticket.Title.Length;
            if (ticket.IsOpen)
            {
                open++;
            }
        }

        double average = tickets.Count == 0 ? 0 : (double)titleLength / tickets.Count;
        return new TicketStats(tickets.Count, open, counts, average);
    }

    private static string FormatList(IReadOnlyList<Ticket> tickets)
    {
        if (tickets.Count == 0)
        {
            return Tickets.Count == 0 ? "No tickets." : "No matches.";
        }

        // StringBuilder avoids allocating a new string on every append.
        var sb = new StringBuilder();
        for (int i = 0; i < tickets.Count; i++)
        {
            if (i > 0)
            {
                sb.AppendLine();
            }

            sb.Append(tickets[i]);
        }

        return sb.ToString();
    }

    private static string FormatSummary(TicketStats stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine(CultureInfo.InvariantCulture, $"Total: {stats.Total}");
        sb.AppendLine(CultureInfo.InvariantCulture, $"Open: {stats.Open}");

        var parts = new List<string>();
        foreach (Priority p in Enum.GetValues<Priority>())
        {
            parts.Add($"{p}={stats.CountByPriority[(int)p]}");
        }

        sb.AppendLine(CultureInfo.InvariantCulture, $"By priority: {string.Join(", ", parts)}");
        sb.Append(CultureInfo.InvariantCulture, $"Average title length: {stats.AverageTitleLength:F1}");
        return sb.ToString();
    }
}
