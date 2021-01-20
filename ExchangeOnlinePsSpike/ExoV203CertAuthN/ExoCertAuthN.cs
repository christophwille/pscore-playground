using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

// Install Module: https://docs.microsoft.com/en-us/powershell/exchange/exchange-online-powershell-v2?view=exchange-ps#install-and-maintain-the-exo-v2-module
//    Note: ps and pwsh have separate installation for modules, ie you could end up with different versions (happened to me...)

namespace ExchangeOnlinePowerShellSpike
{
	public class ExoCertAuthN
	{
		const string demo_certBase64 = ""; // base64 encoded cert w/out password - I did that for simple single-file inclusion in test projects
		const string demo_appId = "yourappguidhere";
		const string demo_organization = "cwie5dev.onmicrosoft.com";

		public static string HardcodedDemo()
		{
			var cert = new X509Certificate2(Convert.FromBase64String(demo_certBase64));
			var exo = new ExoCertAuthN(demo_appId, demo_organization, cert);
			var (err, result) = exo.InMemoryPfxAuthN();
			return result;
		}

		public ExoCertAuthN(string appId, string organization, X509Certificate2 cert)
		{
			AppId = appId;
			Organization = organization;
			Certificate = cert;
		}

		public ExoCertAuthN(string appId, string pfxFile, string pfxPassword, string organization)
		{
			AppId = appId;
			PfxFile = pfxFile;
			PfxPassword = pfxPassword;
			Organization = organization;
		}

		public string AppId { get; }
		public string Organization { get; }
		public X509Certificate2 Certificate { get; }
		public string PfxFile { get; }
		public string PfxPassword { get; }

		private static string scriptedConnectAndExecute = @"Import-Module ExchangeOnlineManagement
Connect-ExchangeOnline -AppId ""theAppId"" -CertificateFilePath ""thePfxFile"" -Organization ""theOrg"" -CertificatePassword (ConvertTo-SecureString -String 'thePfxPassword' -AsPlainText -Force)
Get-EXOMailBox -ResultSize unlimited";

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "MA0074:Avoid implicit culture-sensitive methods", Justification = "POC code")]
		public (string, string) SimpleAuthN()
		{
			if (String.IsNullOrWhiteSpace(PfxFile))
				throw new ArgumentException("Did you use the wrong ctor for this PoC class?");

			using (var ps = PowerShell.Create())
			{
				// Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
				// -> otherwise runtime exception

				string script = scriptedConnectAndExecute
					.Replace("theAppId", AppId).Replace("thePfxFile", PfxFile).Replace("theOrg", Organization).Replace("thePfxPassword", PfxPassword);
				ps.AddScript(script);
				List<PSObject> results = ps.Invoke().ToList();

				return (FlattenErrors(ps), ResultsToSimpleString(results));
			}
		}

		// Tried too, but didn't work (-> Runspace & Pipeline on Runspace):
		//
		//  ps.AddScript("Import-Module ExchangeOnlineManagement");
		//  ps.Invoke();
		//
		//  ps.AddCommand("Import-Module")
		//	     .AddParameter("Name", "ExchangeOnlineManagement");
		public (string, string) InMemoryPfxAuthN()
		{
			if (null == Certificate)
				throw new ArgumentException("Did you use the wrong ctor for this PoC class?");

			// Stage #1

			// netcore31/5 Web App: Cannot load PowerShell snap-in Microsoft.PowerShell.Diagnostics because of the following error: Could not load file or assembly
			//              ==> reason: SMA is not enough... need PS SDK NuGet package
			InitialSessionState iss = InitialSessionState.CreateDefault();
			iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;

			iss.ImportPSModule(new string[] { "ExchangeOnlineManagement" });
			using (var exchangeRunspace = RunspaceFactory.CreateRunspace(iss)) // Using stmts gehen nur in Core... Dreck
			{
				exchangeRunspace.Open();

				using (var pipeLine = exchangeRunspace.CreatePipeline())
				{
					var connectCmd = new Command("Connect-ExchangeOnline");
					connectCmd.Parameters.Add("AppId", AppId);
					connectCmd.Parameters.Add("Organization", Organization);
					connectCmd.Parameters.Add("Certificate", Certificate);

					pipeLine.Commands.Add(connectCmd);

					// NetCore Ex #1: Could not load file or assembly 'Microsoft.Identity.Client, Version=4.23.0.0 
					//              ==> reason: Azure.Identity transitively includes 4.18, thus: manually add 4.23 package
					// NetCore Ex #2: The certificate certificate does not have a private key.
					//              ==> reason: KV... you need to read the certificate as secret to get the private key included
					// NetCore 5 Ex: NotSupportedException: BinaryFormatter serialization and deserialization are disabled within this application. See https://aka.ms/binaryformatter for more information.
					//              ==> reason: see csproj
					pipeLine.Invoke();
					if (pipeLine.Error != null && pipeLine.Error.Count > 0)
					{
						// check error
					}
				}

				// Stage #2
				using (var ps = PowerShell.Create())
				{
					ps.Runspace = exchangeRunspace;

					ps.Commands.AddCommand("Get-EXOMailBox")
						.AddParameter("ResultSize", "unlimited");

					// ps.Commands.AddCommand("Get-MailBox");

					// NetFw Ex #1: System.Management.Automation.CmdletInvocationException System event notifications are not supported under the current context.Server processes, for example, may not support global system event notifications.
					//              ==> reason: Get-EXOMailBox (tested w. System.Management.Automation from GAC - new cmdlets only works with netcore31++)
					List<PSObject> results = ps.Invoke().ToList();

					return (FlattenErrors(ps), ResultsToSimpleString(results));
				}
			}
		}

		public (string, string) InMemoryPfxRunspacePool()
		{
			// IApplicationEnvironment for ASP.NET Core
			string asmPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			string rootFolder = System.IO.Path.GetDirectoryName(asmPath);

			var timer = Stopwatch.StartNew();

			var defaultSessionState = InitialSessionState.CreateDefault();
			defaultSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;
			defaultSessionState.ThrowOnRunspaceOpenError = true;
			defaultSessionState.ImportPSModule(new string[] { "ExchangeOnlineManagement" });
			defaultSessionState.Variables.Add(new SessionStateVariableEntry("exoAppId", AppId, "no description"));
			defaultSessionState.Variables.Add(new SessionStateVariableEntry("exoOrganization", Organization, "no description"));
			defaultSessionState.Variables.Add(new SessionStateVariableEntry("exoCertificate", Certificate, "no description"));
			bool result = defaultSessionState.StartupScripts.Add(System.IO.Path.Combine(rootFolder, "ConnectExO.ps1"));

			using (RunspacePool RsPool = RunspaceFactory.CreateRunspacePool(defaultSessionState))
			{
				RsPool.SetMinRunspaces(1);
				RsPool.SetMaxRunspaces(3);

				RsPool.ThreadOptions = PSThreadOptions.UseNewThread;

				RsPool.Open();

				var ts1 = timer.Elapsed;

				using (var ps = PowerShell.Create())
				{
					ps.RunspacePool = RsPool;

					// ps.Commands.Clear();
					ps.Commands.AddCommand("Get-EXOMailBox")
							.AddParameter("ResultSize", "unlimited");

					// var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);
					List<PSObject> results = ps.Invoke().ToList();

					var ts2 = timer.Elapsed;
					return (FlattenErrors(ps), ResultsToSimpleString(results));
				}
			}
		}

		private static string FlattenErrors(PowerShell ps)
		{
			string errors = "";
			if (ps.Streams.Error.Count > 0)
			{
				errors = "!Errors! " + String.Join(" :: ", ps.StreamErrorsToErrorList());
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
}
