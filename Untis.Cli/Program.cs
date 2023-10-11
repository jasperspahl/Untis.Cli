using System.CommandLine;
using Untis.Cli.Commands;

var root = new RootCommand("Untis Cli is a command line interface for Untis.");
root.AddCommand(new ShowCommand());

return await root.InvokeAsync(args);