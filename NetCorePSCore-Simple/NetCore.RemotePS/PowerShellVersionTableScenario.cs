using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
namespace NetCore.RemotePS
{
	public class PowerShellVersionTableScenario
	{
		public static (string, string) OutputVersionTable()
		{
			using (var ps = PowerShell.Create())
			{
				List<PSObject> results = ps.AddScript("$PSVersionTable | Out-String").Invoke().ToList();

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
	}
}
