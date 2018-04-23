using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

namespace NetCorePSCore
{
	class Program
	{
		// "When will the NuGet package be on nuget.org?" https://github.com/PowerShell/PowerShell/issues/6475

		// Official documentation https://github.com/PowerShell/PowerShell/tree/master/docs/host-powershell
		// SO https://stackoverflow.com/questions/39141914/running-powershell-from-net-core
		// 
		static void Main(string[] args)
		{
			OutputVersionTable();

			Console.WriteLine("Exchange scenarios, please provide an admin user account");
			Console.Write("Admin user: ");
			string admin = Console.ReadLine();
			Console.Write("Password: ");
			string pass = Console.ReadLine();

			SimpleExO(admin, pass);
			// WSManConnectToExO(admin, pass);


			Console.Read();
		}

		static void OutputVersionTable()
		{
			using (var ps = PowerShell.Create())
			{
				List<PSObject> results = ps.AddScript("$PSVersionTable | Out-String").Invoke().ToList();

				if (ps.Streams.Error.Count > 0)
				{
					Console.WriteLine("!Errors! " + String.Join(" :: ", ps.StreamErrorsToErrorList()));
				}

				foreach (var result in results)
				{
					Console.Write(result.ToString());
				}
			}
		}

// Demo PS for connecting to ExO (interactive version)	    
/*
$admin = Read-Host -Prompt "Admin user: "
$pass = Read-Host -Prompt "Password: " -AsSecureString
$cred = New-Object -typename System.Management.Automation.PSCredential -argumentlist $admin, $pass

$session = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri https://outlook.office365.com/powershell-liveid/ -Credential $cred -Authentication  Basic -AllowRedirection
Import-PSSession $session

Get-Mailbox | Out-String
*/
		static void SimpleExO(string admin, string pass)
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

				if (ps.Streams.Error.Count > 0)
				{
					Console.WriteLine("!Errors! " + String.Join(" :: ", ps.StreamErrorsToErrorList()));
				}

				foreach (var result in results)
				{
					Console.Write(result.ToString());
				}
			}
		}

		static void WSManConnectToExO(string admin, string pass)
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

				if (ps.Streams.Error.Count > 0)
				{
					Console.WriteLine("!Errors! " + String.Join(" :: ", ps.StreamErrorsToErrorList()));
				}

				if (!results.Any())
					Console.WriteLine("Identity not found");

				var addressesArray = results.Select(psObject => psObject.Properties["ProxyAddressString"].Value.ToString()).ToList();
				var aliasAddresses = (from address in addressesArray where address.StartsWith("smtp:", StringComparison.InvariantCulture) select address.Substring("smtp:".Length)).ToList();
				var addresses = addressesArray.Where(a => !a.StartsWith("SMTP:")).ToList();

				Console.WriteLine("Alias addresses: " + String.Join(",", aliasAddresses));
				Console.WriteLine("SMTP: " + String.Join(",", addresses));

				pool.Close();
			}
		}
	}
}
