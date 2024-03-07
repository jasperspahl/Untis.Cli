using System.CommandLine;
using Spectre.Console;
using Untis.Cli.Binding;

namespace Untis.Cli.Commands;

public class ShowCommand: Command
{
    public ShowCommand() : base("show", "shows the timetable")
    {
        AddCommand(new ShowDayCommand());
        AddCommand(new ShowWeekCommand());
        
        // TODO: Create an Interaction Flow for showing the timetable
        this.SetHandler((async (client) =>
        {
            var cmd = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What do you want to show?")
                    .AddChoices(this.Subcommands.Select(x => x.Name)));
            switch (cmd)
            {
                case "day":
                    await ShowDayCommand.Run(client, DateTime.Now, false);
                    return;
                case "week":
                    break;
            }
        }), new UntisClientBinder(this));
    }

}