using System.CommandLine;
using Spectre.Console;
using Untis.Cli.Binding;
using Untis.Client;

namespace Untis.Cli.Commands;

public class ShowDayCommand: Command
{
    private readonly Option<DateTime> _dayOption = new(
        aliases: new [] { "-d", "--day"},
        description: "The day to show the timetable for", 
        getDefaultValue: () => DateTime.Now
    );
    private readonly Option<bool> _tomorrowOption = new(
        aliases: new [] { "-t", "--tomorrow"},
        description: "Show the timetable for tomorrow (overrides --day)", 
        getDefaultValue: () => false
    );
    public ShowDayCommand() : base("day", "shows the timetable of a day")
    {
        AddOption(_dayOption);
        AddOption(_tomorrowOption);
        this.SetHandler(Run, new UntisClientBinder(this), _dayOption, _tomorrowOption);
    }

    private async Task Run(IUntisClient client, DateTime date, bool tomorrow)
    {
        
        await AnsiConsole.Status()
        .StartAsync("Logging in...", async ctx =>
        {
            await client.LoginAsync();
            ctx.Status = "Logged in!";
        });
        if (tomorrow)
        {
            date = DateTime.Today.AddDays(1);
        }

        var timetable = await AnsiConsole.Status()
            .StartAsync("Getting timetable...", async ctx =>
            {
                var result = await client.GetOwnTimetableFor(date);
                ctx.Status = "Got timetable!";
                return result.ToList();
            });
        
        timetable.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

        var table = new Table
        {
            Title = new TableTitle($"Stundenplan {date:dd.MM.yyyy}"),
            Border = TableBorder.Rounded
        };
        table.AddColumn(new TableColumn("Uhrzeit").Centered());
        table.AddColumn(new TableColumn("Fach").Centered());
        table.AddColumn("Lehrer");
        table.AddColumn("Raum");
        foreach (var lesson in timetable)
        {
            table.AddRow(
                $"[green]{lesson.StartTime:00:00}[/]-[red]{lesson.EndTime:00:00}[/]",
                lesson.Su.First().Name,
                lesson.Te.First().LongName,
                lesson.Ro.First().Name
            );
        }
        
        AnsiConsole.Write(table);
    }
}