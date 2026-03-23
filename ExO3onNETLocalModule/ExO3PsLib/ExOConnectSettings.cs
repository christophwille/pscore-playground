namespace ExO3PsLib
{
    public interface IPowerShellModuleSettings
    {
        string ModuleBasePath { get; set; }
        string ModRelPathExchangeOnlineManagement { get; set; }
        string ModRelPathPackageManagement { get; set; }
        string ModRelPathPowerShellGet { get; set; }
    }

    public class ExOConnectSettings : IPowerShellModuleSettings
    {
        public string PfxPath { get; set; }
        public string PfxPassword { get; set; }
        public string AppId { get; set; }
        public string Organization { get; set; }

        public string ModuleBasePath { get; set; }
        public string ModRelPathExchangeOnlineManagement { get; set; }
        public string ModRelPathPackageManagement { get; set; }
        public string ModRelPathPowerShellGet { get; set; }
    }
}
