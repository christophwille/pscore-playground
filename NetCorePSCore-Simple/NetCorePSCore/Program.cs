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
