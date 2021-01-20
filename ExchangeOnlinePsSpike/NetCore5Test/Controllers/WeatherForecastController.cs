using ExchangeOnlinePowerShellSpike;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore5Test.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{

		private readonly ILogger<WeatherForecastController> _logger;

		// The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
		static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public string Get()
		{
			string result = "";

			try
			{
				result = ExoCertAuthN.HardcodedDemo();
			}
			catch (Exception ex)
			{
				result = ex.ToString();
			}

			return result;
		}
	}
}
