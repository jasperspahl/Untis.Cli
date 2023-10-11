using System.Diagnostics;

namespace Untis.Playground;

public class PassHelper
{
    public static string GetPassword(string key)
    {
        var proc = new Process();
        proc.StartInfo.FileName = "pass";
        proc.StartInfo.Arguments = $"show {key}";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        var output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        return output.Trim();
    }
}