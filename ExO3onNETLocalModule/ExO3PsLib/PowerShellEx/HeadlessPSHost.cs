using System.Globalization;
using System.Management.Automation.Host;

namespace ExO3PsLib.PowerShellEx
{
    // https://learn.microsoft.com/en-us/powershell/scripting/developer/hosting/host02-sample?view=powershell-7.4

    internal class HeadlessPSHost : PSHost
    {
        public override string Name => "Custom Headless PSHost";

        public override Version Version => new Version(1, 0, 0, 0);

        private Guid myId = Guid.NewGuid();
        public override Guid InstanceId => this.myId;

        private HeadlessPSHostUserInterface myInterface = new HeadlessPSHostUserInterface();
        public override PSHostUserInterface UI => this.myInterface;

        private CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;
        private CultureInfo originalUICultureInfo = Thread.CurrentThread.CurrentUICulture;

        public override CultureInfo CurrentCulture => this.originalCultureInfo;

        public override CultureInfo CurrentUICulture => this.originalUICultureInfo;

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
            return;
        }

        public override void NotifyEndApplication()
        {
            return;
        }

        public override void SetShouldExit(int exitCode)
        {
            return;
        }
    }
}
