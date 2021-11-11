using Microsoft.Extensions.Options;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ExOonNet6;

public interface IExchangeOnlineService
{
    (string errors, string result) GetExoMailbox();
}

public class ExchangeOnlineService : IExchangeOnlineService
{
    private readonly ILogger<ExchangeOnlineService> _logger;

    public string AppId { get; }
    public string Organization { get; }
    public X509Certificate2 Certificate { get; }

    public ExchangeOnlineService(IOptions<ExOSettingsOptions> options, ILogger<ExchangeOnlineService> logger)
    {
        AppId = options.Value.AppId;
        Organization = options.Value.Organization;

        string pfxFile = options.Value.PfxPath;
        string pfxPwd = options.Value.PfxPassword;
        Certificate = new X509Certificate2(pfxFile, pfxPwd);

        _logger = logger;
    }

    public (string errors, string result) GetExoMailbox()
    {
        // Stage #1
        InitialSessionState iss = InitialSessionState.CreateDefault();
        iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;
        iss.ImportPSModule(new string[] { "ExchangeOnlineManagement" });

        using var exchangeRunspace = RunspaceFactory.CreateRunspace(iss);
        exchangeRunspace.Open();

        using (var pipeLine = exchangeRunspace.CreatePipeline())
        {
            var connectCmd = new Command("Connect-ExchangeOnline");
            connectCmd.Parameters.Add("AppId", AppId);
            connectCmd.Parameters.Add("Organization", Organization);
            connectCmd.Parameters.Add("Certificate", Certificate);

            pipeLine.Commands.Add(connectCmd);

            /*
            Inner Exception 1:
            PSRemotingDataStructureException: An error has occurred which PowerShell cannot handle. A remote session might have ended.

            Inner Exception 2:
            NotSupportedException: BinaryFormatter serialization and deserialization are disabled within this application. See https://aka.ms/binaryformatter for more information.
            */
            // See .csproj for EnableUnsafeBinaryFormatterSerialization, otherwise .Invoke throws above exception.
            pipeLine.Invoke();
            if (pipeLine.Error != null && pipeLine.Error.Count > 0)
            {
                // check error
            }
        }

        // Stage #2
        using var ps = PowerShell.Create();
        ps.Runspace = exchangeRunspace;

        ps.Commands.AddCommand("Get-EXOMailBox").AddParameter("ResultSize", "unlimited");

        List<PSObject> results = ps.Invoke().ToList();

        return (FlattenErrors(ps), ResultsToSimpleString(results));
    }

    private static string FlattenErrors(PowerShell ps)
    {
        string errors = "";
        if (ps.Streams.Error.Count > 0)
        {
            errors = "!Errors! " + String.Join(" :: ", ps.Streams.Error.Select(error => error.ToString()).ToList());
        }

        return errors;
    }

    private static string ResultsToSimpleString(List<PSObject> results)
    {
        StringBuilder stb = new StringBuilder();
        foreach (var result in results)
        {
            stb.AppendLine(result.ToString());
        }

        return stb.ToString();
    }
}
