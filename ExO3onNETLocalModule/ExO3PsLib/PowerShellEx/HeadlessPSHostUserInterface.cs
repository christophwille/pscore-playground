using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace ExO3PsLib.PowerShellEx
{
    internal class HeadlessPSHostUserInterface : PSHostUserInterface
    {
        private HeadlessPSHostRawUserInterface myRawUi = new HeadlessPSHostRawUserInterface();
        public override PSHostRawUserInterface RawUI => this.myRawUi;

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }

        public override void Write(string value)
        {
            System.Console.Write(value);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            System.Console.Write(value);
        }

        public override void WriteDebugLine(string message)
        {
            Console.WriteLine(String.Format(
                                CultureInfo.CurrentCulture,
                                "DEBUG: {0}",
                                message));
        }

        public override void WriteErrorLine(string value)
        {
            Console.WriteLine(String.Format(
                                      CultureInfo.CurrentCulture,
                                      "ERROR: {0}",
                                      value));
        }

        public override void WriteLine(string value)
        {
            System.Console.WriteLine();
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        public override void WriteVerboseLine(string message)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        public override void WriteWarningLine(string message)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }
    }
}
