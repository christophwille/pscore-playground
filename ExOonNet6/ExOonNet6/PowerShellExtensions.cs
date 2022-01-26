using System.Management.Automation;

namespace ExOonNet6;

public static class PowerShellExtensions
{
    public static async Task<List<PSObject>> InvokeAsyncConditionally(this PowerShell ps, bool invokeAsync = true)
    {
        if (invokeAsync)
        {
            var dataCollection = await ps.InvokeAsync();
            return dataCollection.ToList();
        }

        return ps.Invoke().ToList();
    }

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
