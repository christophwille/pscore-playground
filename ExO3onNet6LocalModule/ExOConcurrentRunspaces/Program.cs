using ExO3PsLib;
using Microsoft.Extensions.Configuration;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography.X509Certificates;
using System.Text;

const string modulePath = "D:\\GitWorkspace\\_exo3module_unpacked\\3.1.0\\ExchangeOnlineManagement.psd1";
const string pfxbasepath = "D:\\GitWorkspace\\";

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

IConfigurationRoot configuration = builder.Build();

string orgs = configuration["ExOOrganizations"];
string appIds = configuration["ExOAppIds"];
var exoOrgs = orgs.Split(",");
var exoApps = appIds.Split(",");

if (exoOrgs.Length != exoApps.Length)
{
    throw new Exception("orgs and apps don't match up");
}

Dictionary<string, RunspacePool> perOrgRunspacePools = new();
var factory = new ExchangeOnlinePowerShellFactory(null);

for (int i = 0; i < exoOrgs.Length; i++)
{
    string org = exoOrgs[i];
    string appId = exoApps[i];

    RunspacePool pool = factory.OpenPool(appId, org, new X509Certificate2(pfxbasepath + org + ".pfx"), modulePath);
    perOrgRunspacePools.Add(org, pool);
}

// Using Get-Mailbox
foreach (string org in perOrgRunspacePools.Keys)
{
    using (var ps = CreatePowerShellForRunspacePool(perOrgRunspacePools[org]))
    {
        var (error, result) = GetMailbox(ps);
        if (!result.Contains(org))
        {
            System.Diagnostics.Debugger.Launch();
        }
    }
}

// Using Get-EXOMailbox
foreach (string org in perOrgRunspacePools.Keys)
{
    using (var ps = CreatePowerShellForRunspacePool(perOrgRunspacePools[org]))
    {
        var (error, result) = GetExoMailbox(ps);
        if (!result.Contains(org))
        {
            System.Diagnostics.Debugger.Launch();
        }
    }
}

PowerShell CreatePowerShellForRunspacePool(RunspacePool pool)
{
    var ps = PowerShell.Create();
    ps.RunspacePool = pool;
    return ps;
}

(string error, string result) GetExoMailbox(PowerShell ps)
{
    var propertySets = new[] { "Minimum", "Custom" };
    ps.AddCommand("Get-EXOMailbox");
    ps.AddParameter("RecipientTypeDetails", "SharedMailbox");
    ps.AddParameter("ResultSize", "Unlimited");
    ps.AddParameter("PropertySets", propertySets);
    ICollection<PSObject> results = ps.Invoke();

    var err = ps.StreamsErrorToString();
    return (err, ParsePSmtp(results));
}

(string error, string result) GetMailbox(PowerShell ps)
{
    ps.AddCommand("Get-Mailbox");
    ps.AddParameter("RecipientTypeDetails", "SharedMailbox");
    ps.AddParameter("ResultSize", "Unlimited");
    ICollection<PSObject> results = ps.Invoke();

    var err = ps.StreamsErrorToString();
    return (err, ParsePSmtp(results));
}

string ParsePSmtp(ICollection<PSObject> results)
{
    StringBuilder stb = new StringBuilder();
    foreach (var result in results)
    {
        stb.AppendLine(result.Members["primarysmtpaddress"].Value.ToString());
    }
    return stb.ToString();
}