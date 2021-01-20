using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace ExchangeOnlinePowerShellSpike
{
	public static class ErrorStreamExtensions
	{
		public static List<string> StreamErrorsToErrorList(this PowerShell ps)
		{
			return ps.Streams.Error.Select(error => error.ToString()).ToList();
		}
	}
}
