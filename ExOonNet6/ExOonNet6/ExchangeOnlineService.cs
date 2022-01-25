using Microsoft.Extensions.Options;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ExOonNet6;

public record ExOPfx(string PfxBase64, string PfxPassword);
public record ExOResult(string Errors, string Result, long TimeToConnect, long TimeAfterCmds, long TimeTotal);

public interface IExchangeOnlineService
{
    ExOResult GetExoMailbox();
    ExOResult GetExoMailboxWithPfx(ExOPfx pfxInfo);
    ExOPfx GetPfxInfo();
}

public class ExchangeOnlineService : IExchangeOnlineService
{
    private readonly ExOSettingsOptions _options;
    private readonly ILogger<ExchangeOnlineService> _logger;

    public string AppId { get; }
    public string Organization { get; }
    public X509Certificate2 Certificate { get; private set; }

    public ExchangeOnlineService(IOptions<ExOSettingsOptions> options, ILogger<ExchangeOnlineService> logger)
    {
        _options = options.Value;
        AppId = _options.AppId;
        Organization = _options.Organization;

        string pfxFile = _options.PfxPath;
        string pfxPwd = _options.PfxPassword;
        try
        {
            Certificate = new X509Certificate2(pfxFile, pfxPwd);
        }
        catch (Exception e)
        {
            logger.LogError(exception: e, "Failed to load PFX file");
        }

        _logger = logger;
    }

    public ExOPfx GetPfxInfo()
    {
        byte[] pfxBytes = System.IO.File.ReadAllBytes(_options.PfxPath);
        string base64Pfx = System.Convert.ToBase64String(pfxBytes);
        return new ExOPfx(base64Pfx, _options.PfxPassword);
    }

    public ExOResult GetExoMailbox()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long elapsedConnect, elapsedCmds, elapsedTotal = 0;

        // Stage #1
        InitialSessionState iss = InitialSessionState.CreateDefault();

        // NOTE: No longer necessary anyways in 2.0.6, but would fail in Docker Ubuntu
        // System.PlatformNotSupportedException Message = Operation is not supported on this platform.
        // StackTrace: at System.Management.Automation.Internal.SecuritySupport.SetExecutionPolicy
        // iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;

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

            try
            {
                pipeLine.Invoke();
            }
            catch (Exception e)
            {
                return new ExOResult(e.ToString(), "", 0, 0, 0);
            }

            elapsedConnect = sw.ElapsedMilliseconds;
            if (pipeLine.Error != null && pipeLine.Error.Count > 0)
            {
                throw new NotImplementedException(); // Error handing code below is not tested
#pragma warning disable CS0162 // Unreachable code detected
                var errors = pipeLine.Error.ReadToEnd();
                var errStringified = "!Errors! " + String.Join(" :: ", errors.Select(error => error.ToString()).ToList());
                return new ExOResult(errStringified, "", elapsedConnect, 0, 0);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        // Stage #2
        using var ps = PowerShell.Create();
        ps.Runspace = exchangeRunspace;

        ps.Commands.AddCommand("Get-EXOMailBox").AddParameter("ResultSize", "unlimited");

        List<PSObject> results = ps.Invoke().ToList();
        elapsedCmds = sw.ElapsedMilliseconds;
        var err = FlattenErrors(ps);
        var result = ResultsToSimpleString(results);

        // https://docs.microsoft.com/en-us/powershell/module/exchange/disconnect-exchangeonline?view=exchange-ps
        ps.Commands.Clear();
        ps.Commands.AddCommand("Disconnect-ExchangeOnline").AddParameter("Confirm", false);
        var disconnectResult = ps.Invoke().ToList();
        sw.Stop();
        elapsedTotal = sw.ElapsedMilliseconds;
        var disconnectErr = FlattenErrors(ps);

        return new ExOResult(err, result, elapsedConnect, elapsedCmds, elapsedTotal);
    }

    public ExOResult GetExoMailboxWithPfx(ExOPfx pfxInfo)
    {
        byte[] data = Convert.FromBase64String(pfxInfo.PfxBase64);
        Certificate = new X509Certificate2(data, pfxInfo.PfxPassword);

        return GetExoMailbox();
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
