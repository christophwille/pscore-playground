using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;

namespace NetCore.RemotePS
{
	public class RemoteExchangePowerShellScenarios
	{
		// Demo PS for connecting to ExO (interactive version)	    
		/*
		$admin = Read-Host -Prompt "Admin user: "
		$pass = Read-Host -Prompt "Password: " -AsSecureString
		$cred = New-Object -typename System.Management.Automation.PSCredential -argumentlist $admin, $pass

		$session = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri https://outlook.office365.com/powershell-liveid/ -Credential $cred -Authentication  Basic -AllowRedirection
		Import-PSSession $session

		Get-Mailbox | Out-String
		*/
		public static (string, string) SimpleExO(string admin, string pass)
		{
			string pscmd = @"Set-ExecutionPolicy Unrestricted
$admin = ""REPLACEUSER""
$pass = ConvertTo-SecureString ""REPLACEPASS"" -AsPlainText -Force
$cred = New-Object -Typename System.Management.Automation.PSCredential -argumentlist $admin,$pass

$session = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri https://outlook.office365.com/powershell-liveid/ -Credential $cred -Authentication  Basic -AllowRedirection
Import-PSSession $session

Get-Mailbox | Out-String".Replace("REPLACEUSER", admin).Replace("REPLACEPASS", pass);

			using (var ps = PowerShell.Create())
			{
				List<PSObject> results = ps.AddScript(pscmd).Invoke().ToList();

				string errors = "";
				StringBuilder stb = new StringBuilder();
				if (ps.Streams.Error.Count > 0)
				{
					errors = "!Errors! " + String.Join(" :: ", ps.StreamErrorsToErrorList());
				}

				foreach (var result in results)
				{
					stb.AppendLine(result.ToString());
				}

				return (errors, stb.ToString());
			}
		}

		public static (string, string) WSManConnectToExO(string admin, string pass)
		{
			using (var ps = PowerShell.Create())
			{
				var secureString = new SecureString();
				pass.ToCharArray().ToList().ForEach(p => secureString.AppendChar(p));
				var credential = new PSCredential(admin, secureString);

				var connectionInfo = new WSManConnectionInfo(
					new Uri("https://outlook.office365.com/powershell-liveid/"),
					"http://schemas.microsoft.com/powershell/Microsoft.Exchange",
					credential);
				connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;
				connectionInfo.MaximumConnectionRedirectionCount = 2;

				var pool = RunspaceFactory.CreateRunspacePool(1, 5, connectionInfo);
				pool.ThreadOptions = PSThreadOptions.UseNewThread;
				pool.Open();

				ps.RunspacePool = pool;

				ps.AddCommand("Get-Mailbox");
				ps.AddParameter("Identity", admin);

				ps.AddCommand("Select-Object");
				ps.AddParameter("ExpandProperty", "EmailAddresses");
				List<PSObject> results = ps.Invoke().ToList();

				string errors = "";
				if (ps.Streams.Error.Count > 0)
				{
					errors = "!Errors! " + String.Join(" :: ", ps.StreamErrorsToErrorList());
				}

				if (!results.Any())
					errors += "Identity not found";

				var addressesArray = results.Select(psObject => psObject.Properties["ProxyAddressString"].Value.ToString()).ToList();
				var aliasAddresses = (from address in addressesArray where address.StartsWith("smtp:", StringComparison.InvariantCulture) select address.Substring("smtp:".Length)).ToList();
				var addresses = addressesArray.Where(a => !a.StartsWith("SMTP:")).ToList();

				StringBuilder stb = new StringBuilder();
				stb.AppendLine("Alias addresses: " + String.Join(",", aliasAddresses));
				stb.AppendLine("SMTP: " + String.Join(",", addresses));

				pool.Close();

				return (errors, stb.ToString());
			}
		}
	}
}
