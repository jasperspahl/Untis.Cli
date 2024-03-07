using System.CommandLine;
using Spectre.Console;
using Untis.Cli.Binding;
using Untis.Client;
using Untis.Client.Contracts;

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

    internal static async Task Run(IUntisClient client, DateTime date, bool tomorrow)
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
        Dictionary<string, List<Lesson>> lessonsByTime = new();
        foreach (var lesson in timetable)
        {
            var time = $"{lesson.StartTime:00:00}-{lesson.EndTime:00:00}";
            if (!lessonsByTime.ContainsKey(time))
            {
                lessonsByTime.Add(time, new List<Lesson>());
            }
            lessonsByTime[time].Add(lesson);
        }

        var table = new Table
        {
            Title = new TableTitle($"Stundenplan {date:dd.MM.yyyy}"),
            Border = TableBorder.Rounded
        };
        table.AddColumn(new TableColumn("Uhrzeit").Centered());
        table.AddColumn(new TableColumn("Fach").Centered());
        table.AddColumn("Lehrer");
        table.AddColumn("Raum");

        foreach (var (_, lessons) in lessonsByTime)
        {

            var subjects = lessons.Select(x => new Markup($"[{MapToColor(x.Code)}]{x.Su.First().Name}[/]"));

            var teachers = lessons.Select(x => new Markup($"[{MapToColor(x.Code)}]{x.Te.First().LongName}[/]"));
            var room = lessons.Select(x => new Markup($"[{MapToColor(x.Code)}]{x.Ro.First().Name}[/]"));
            
            table.AddRow(
                new Markup($"[green]{lessons.First().StartTime:00:00}[/]-[red]{lessons.First().EndTime:00:00}[/]"),
                new Columns(subjects),
                new Columns(teachers),
                new Columns(room)
            );
        }
        
        AnsiConsole.Write(table);
        return;
    }

    private static string MapToColor(string? x) =>
        x switch
        {
            "irregular" => "yellow",
            "cancelled" => "strikethrough red",
            _ => "white"
        };
    
}