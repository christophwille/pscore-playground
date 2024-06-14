using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ExO3PsLib;

public class ExchangeOnlineService : IExchangeOnlineService
{
    private readonly ExOConnectSettings _options;
    private readonly IExchangeOnlinePowerShellFactory _exoPsFactory;
    private readonly ILogger<ExchangeOnlineService> _logger;

    public X509Certificate2 Certificate { get; private set; }

    public ExchangeOnlineService(IOptions<ExOConnectSettings> connectionSettings, IExchangeOnlinePowerShellFactory exoPsFactory, ILogger<ExchangeOnlineService> logger)
    {
        _options = connectionSettings.Value;
        _exoPsFactory = exoPsFactory;
        _logger = logger;

        try
        {
            Certificate = new X509Certificate2(_options.PfxPath, _options.PfxPassword);
        }
        catch (Exception e)
        {
            _logger.LogError(exception: e, "Failed to load PFX file");
        }
    }

    public ExOPfx GetPfxInfo()
    {
        byte[] pfxBytes = System.IO.File.ReadAllBytes(_options.PfxPath);
        string base64Pfx = System.Convert.ToBase64String(pfxBytes);
        return new ExOPfx(base64Pfx, _options.PfxPassword);
    }

    public async Task<ExOResult> GetExoMailbox()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long elapsedConnect = 0, elapsedCmds = 0, elapsedTotal = 0;
        string err = string.Empty, result = string.Empty;

        try
        {
            using var ps = _exoPsFactory.ConnectViaPool(_options.AppId, _options.Organization, Certificate, _options.ModulePath);
            elapsedConnect = sw.ElapsedMilliseconds;

            // If an exception happens here, then the %temp%/tmpEXO_ folder sticks around
            ps.Commands.AddCommand("Get-EXOMailBox").AddParameter("ResultSize", "unlimited");
            ICollection<PSObject> results = await ps.InvokeAsync().ConfigureAwait(false); // DO NOT materialize to a List<T>

            elapsedCmds = sw.ElapsedMilliseconds;
            err = ps.StreamsErrorToString();
            result = ResultsToSimpleString(results);

            // https://docs.microsoft.com/en-us/powershell/module/exchange/disconnect-exchangeonline?view=exchange-ps
            ps.Commands.Clear();
            ps.Commands.AddCommand("Disconnect-ExchangeOnline").AddParameter("Confirm", false);
            var disconnectResult = await ps.InvokeAsync().ConfigureAwait(false);
            err = ps.StreamsErrorToString();
        }
        catch (Exception ex)
        {
            err = ex.ToString();
        }
        finally
        {
            sw.Stop();
            elapsedTotal = sw.ElapsedMilliseconds;
        }

        return new ExOResult(err, result, elapsedConnect, elapsedCmds, elapsedTotal);
    }

    public Task<ExOResult> GetExoMailboxWithPfx(ExOPfx pfxInfo)
    {
        byte[] data = Convert.FromBase64String(pfxInfo.PfxBase64);
        Certificate = new X509Certificate2(data, pfxInfo.PfxPassword);

        return GetExoMailbox();
    }

    public static string ResultsToSimpleString(ICollection<PSObject> results)
    {
        StringBuilder stb = new StringBuilder();
        foreach (var result in results)
        {
            stb.AppendLine(result.ToString());
        }

        return stb.ToString();
    }
}
