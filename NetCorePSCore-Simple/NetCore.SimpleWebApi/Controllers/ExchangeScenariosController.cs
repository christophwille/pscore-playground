using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NetCore.RemotePS;

namespace NetCore.SimpleWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExchangeScenariosController : ControllerBase
	{
		[HttpGet]
		public string Get(string username, string password)
		{
			try
			{
				StringBuilder stb = new StringBuilder();
				var (errS1, resultS1) = RemoteExchangePowerShellScenarios.SimpleExO(username, password);
				stb.AppendLine($"Call#1, Errors: {errS1}, Result: {resultS1}");

				var (errS2, resultS2) = RemoteExchangePowerShellScenarios.WSManConnectToExO(username, password);
				stb.AppendLine($"Call#1, Errors: {errS2}, Result: {resultS2}");

				return stb.ToString();
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
	}
}