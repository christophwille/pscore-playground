using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace ExchangeOnlinePowerShellSpike
{
	public static class PowerShellVersionTableScenario
	{
		public static string OutputVersionTable()
		{
			using (var ps = PowerShell.Create())
			{
				List<PSObject> results = ps.AddScript("$PSVersionTable | Out-String").Invoke().ToList();

				StringBuilder stb = new StringBuilder();
				foreach (var result in results)
				{
					stb.AppendLine(result.ToString());
				}

				return stb.ToString();
			}
		}
	}
}