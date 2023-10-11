using Spectre.Console;
using Untis.Client;
using Untis.Playground;

var password = AnsiConsole.Status().Start("Getting Password...", ctx =>
{
    var result = PassHelper.GetPassword("www/jasper.spahl@its-stuttgart.de");
    ctx.Status($"Got Password!");
    return result;
});
using var client = new UntisClientBasic("IT-Schule Stuttgart", "jasper.spahl", password, "https://mese.webuntis.com", "Untis.Playground");
await AnsiConsole.Status()
    .StartAsync("Logging in...", async ctx =>
    {
        await client.LoginAsync();
        ctx.Status = "Logged in!";
    });
var date = new DateTime(2023, 10, 5);
var timetable =
await AnsiConsole.Status()
    .StartAsync("Getting timetable...", async ctx =>
    {
        var result =  await client.GetOwnTimetableFor(date);
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
