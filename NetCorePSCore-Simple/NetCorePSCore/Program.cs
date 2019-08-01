using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using NetCore.RemotePS;

namespace NetCorePSCore
{
	class Program
	{
		// https://www.nuget.org/packages/Microsoft.PowerShell.SDK/

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

			var (errS1, resultS1) = RemoteExchangePowerShellScenarios.SimpleExO(admin, pass);
			Console.WriteLine($"Call#1, Errors: {errS1}, Result: {resultS1}");

			var (errS2, resultS2) = RemoteExchangePowerShellScenarios.WSManConnectToExO(admin, pass);
			Console.WriteLine($"Call#1, Errors: {errS2}, Result: {resultS2}");

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
	}
}
