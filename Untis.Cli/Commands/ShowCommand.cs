using System.CommandLine;
namespace Untis.Cli.Commands;

public class ShowCommand: Command
{
    public ShowCommand() : base("show", "shows the timetable")
    {
        AddCommand(new ShowDayCommand());
        AddCommand(new ShowWeekCommand());
        
        // TODO: Create an Interaction Flow for showing the timetable
    }

}