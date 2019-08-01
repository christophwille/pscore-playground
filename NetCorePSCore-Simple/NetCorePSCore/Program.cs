using System;
using NetCore.RemotePS;

namespace NetCorePSCore
{
	class Program
	{
		// https://www.nuget.org/packages/Microsoft.PowerShell.SDK/
		//
		// Official documentation https://github.com/PowerShell/PowerShell/tree/master/docs/host-powershell
		// SO https://stackoverflow.com/questions/39141914/running-powershell-from-net-core
		// 
		static void Main(string[] args)
		{
			var (errVT, resultVT) = PowerShellVersionTableScenario.OutputVersionTable();
			Console.WriteLine($"Version table, Errors: {errVT}{Environment.NewLine}{resultVT}");

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
	}
}
