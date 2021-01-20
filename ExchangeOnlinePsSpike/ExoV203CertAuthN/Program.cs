using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ExchangeOnlinePowerShellSpike
{
	class Program
	{
		const string onDiskPfxLocation = @"C:\YourDirectory\YourExchangeApp.pfx";
		const string pfxPassword = "yourpasswordhere";
		const string appId = "yourappguidhere";
		const string organization = "cwie5dev.onmicrosoft.com";

		const string kvUrl = "https://testkvcwi.vault.azure.net/";
		const string kvCertSecretName = "yourexchangeapp";

		static async Task Main(string[] args)
		{
			// TestOutputPsVersionTable();
			// TestSimpleAuthN();

			X509Certificate2 cert = null;
			// cert = await TestGetCertFromKeyVault();

			cert = new X509Certificate2(File.ReadAllBytes(onDiskPfxLocation), pfxPassword);

			var exo = new ExoCertAuthN(appId,
				organization,
				cert);

			// var (err1, result1) = exo.InMemoryPfxRunspacePool();

			var (err, result) = exo.InMemoryPfxAuthN();
			Console.WriteLine(result);
		}

		static void TestOutputPsVersionTable()
		{
			string vTable = PowerShellVersionTableScenario.OutputVersionTable();
			Console.WriteLine(vTable);
		}

		static void TestSimpleAuthN()
		{
			var exo = new ExoCertAuthN(appId, onDiskPfxLocation, pfxPassword, organization);
			var (err, result) = exo.SimpleAuthN();
		}

		static async Task<X509Certificate2> TestGetCertFromKeyVault()
		{
			// var testSecret = await kvsvc.GetSecretAsync("bootsize");

			var kvsvc = new KeyVaultService(kvUrl);
			X509Certificate2 cert = await kvsvc.GetCertificateWithPrivateKeyAsync(kvCertSecretName);

			return cert;
		}
	}
}