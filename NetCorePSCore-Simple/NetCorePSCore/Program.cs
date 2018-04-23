using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

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
	}
}
