using System.CommandLine;
using Spectre.Console;

namespace Untis.Cli.Commands;

public class ShowWeekCommand: Command
{
    public ShowWeekCommand() : base("week", "shows the timetable of a week")
    {
        this.SetHandler(() => AnsiConsole.MarkupLine("[green]Week[/]"));
    }
}