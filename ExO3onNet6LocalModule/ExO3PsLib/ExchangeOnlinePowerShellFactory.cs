using Microsoft.Extensions.Logging;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography.X509Certificates;

namespace ExO3PsLib
{
    public class ExchangeOnlinePowerShellFactory : IExchangeOnlinePowerShellFactory
    {
        private readonly ILogger<ExchangeOnlinePowerShellFactory> _logger;

        public ExchangeOnlinePowerShellFactory(ILogger<ExchangeOnlinePowerShellFactory> logger)
        {
            _logger = logger;
        }

        private InitialSessionState CreateInitialSessionState(string modulePath)
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted; // otherwise: ".. cannot be loaded because running scripts is disabled on this system"
            iss.ImportPSModule(new string[] { modulePath }); // would be "ExchangeOnlineManagement" for installed PS module

            return iss;
        }

        public PowerShell Connect(string appId, string organization, X509Certificate2 certificate, string modulePath)
        {
            var iss = CreateInitialSessionState(modulePath);

            var exchangeRunspace = RunspaceFactory.CreateRunspace(iss);
            exchangeRunspace.Open();

            using (var pipeLine = exchangeRunspace.CreatePipeline())
            {
                var connectCmd = new Command("Connect-ExchangeOnline");
                connectCmd.Parameters.Add("AppId", appId);
                connectCmd.Parameters.Add("Organization", organization);
                connectCmd.Parameters.Add("Certificate", certificate);

                pipeLine.Commands.Add(connectCmd);

                try
                {
                    pipeLine.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Connecting to Exchange Online failed");
                    throw;
                }

                if (pipeLine.Error != null && pipeLine.Error.Count > 0)
                {
                    throw new NotImplementedException(); // Error handing code below is not tested
#pragma warning disable CS0162 // Unreachable code detected
                    var errors = pipeLine.Error.ReadToEnd();
                    var errStringified = "!Errors! " + String.Join(" :: ", errors.Select(error => error.ToString()).ToList());
                    // TODO: Write to _logger
#pragma warning restore CS0162 // Unreachable code detected
                }
            }

            var ps = PowerShell.Create();
            ps.Runspace = exchangeRunspace;

            return ps;
        }
    }
}
