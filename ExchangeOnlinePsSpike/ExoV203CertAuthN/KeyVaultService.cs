using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ExchangeOnlinePowerShellSpike
{
	// **Latest** KeyVault SDK: Azure.Security.KeyVault.Certificates
	// https://github.com/Azure/azure-sdk-for-net/blob/2d11c6664e68c145a988729e6598449bba130694/sdk/keyvault/Azure.Security.KeyVault.Certificates/README.md
	//   or https://azuresdkdocs.blob.core.windows.net/$web/dotnet/Azure.Security.KeyVault.Certificates/4.1.0/api/index.html
	// https://docs.microsoft.com/en-us/dotnet/api/azure.security.keyvault.certificates?view=azure-dotnet
	public class KeyVaultService
	{
		private readonly CertificateClient _certClient = null;
		private readonly SecretClient _secretClient = null;

		public KeyVaultService(string keyVaultUrl)
		{
			_certClient = new CertificateClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());
			_secretClient = new SecretClient(vaultUri: new Uri(keyVaultUrl), credential: new DefaultAzureCredential());
		}

		// https://azuresdkdocs.blob.core.windows.net/$web/dotnet/Azure.Security.KeyVault.Secrets/4.1.0/api/index.html
		public async Task<string> GetSecretAsync(string secretName)
		{
			var secret = await _secretClient.GetSecretAsync(secretName);
			return secret.Value.Value;
		}

		public async Task<byte[]> GetCertificateBytesAsync(string certName)
		{
			KeyVaultCertificateWithPolicy certificateWithPolicy = await _certClient.GetCertificateAsync(certName);
			return certificateWithPolicy.Cer;
		}

		public async Task<X509Certificate2> GetCertificateAsync(string certName)
		{
			var certBytes = await GetCertificateBytesAsync(certName);
			return new X509Certificate2(certBytes);
			// var certBytesAsBase64String = Convert.ToBase64String(x509);
		}

		public async Task<X509Certificate2> GetCertificateWithPrivateKeyAsync(string certName)
		{
			var certBase64Encoded = await GetSecretAsync(certName);
			return new X509Certificate2(Convert.FromBase64String(certBase64Encoded));
			// var certBytesAsBase64String = Convert.ToBase64String(x509);
		}
	}
}
