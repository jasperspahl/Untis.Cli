using System.Diagnostics;

namespace Untis.Cli.Helpers;

public static class ShellHelper
{
    public static string RunCommand(string cmd)
    {
        // Split the command into the filename and the arguments
        var spaceIndex = cmd.IndexOf(' ');
        var proc = new Process();
        proc.StartInfo.FileName = cmd.Substring(0, spaceIndex);
        proc.StartInfo.Arguments = cmd.Substring(spaceIndex + 1);
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        var output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        return output.Trim();
    }
}