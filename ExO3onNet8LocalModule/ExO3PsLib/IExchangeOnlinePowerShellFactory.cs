using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;

namespace ExO3PsLib
{
    public interface IExchangeOnlinePowerShellFactory
    {
        PowerShell Connect(string appId, string organization, X509Certificate2 certificate, string modulePath);
        PowerShell ConnectViaPool(string appId, string organization, X509Certificate2 certificate, string modulePath);
    }
}