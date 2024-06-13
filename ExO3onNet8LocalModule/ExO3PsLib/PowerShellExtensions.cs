using System.Management.Automation;

namespace ExO3PsLib;

public static class PowerShellExtensions
{
    public static string StreamsErrorToString(this PowerShell ps)
    {
        string errors = "";
        if (ps.Streams.Error.Count > 0)
        {
            errors = "!Errors! " + String.Join(" :: ", ps.Streams.Error.Select(error => error.ToString()).ToList());
        }

        return errors;
    }
}
