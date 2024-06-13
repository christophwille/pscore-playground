# Sample loading extracted ExchangeOnline module

https://www.powershellgallery.com/packages/ExchangeOnlineManagement/3.0.0

Manual Download / Download the raw nupkg file

* Save with extension .zip
* Make sure to go to Properties / check the Unblock and click Apply (otherwise won't execute)
* Extract to folder (you could delete _rels, package, xml + nuspec in root)
* Set appsettings.json / ExOConnectSettings / ModulePath, eg `"c:\\yourextracfolder\\ExchangeOnlineManagement.psd1"`

appSettings.Development.json sample

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ExOConnectSettings": {
    "PfxPath": "D:\\GitWorkspace\\sample.pfx",
    "PfxPassword": "",
    "AppId": "guidforpfxhere",
    "Organization": "youronmsdomainhere.onmicrosoft.com",
    "ModulePath": "D:\\GitWorkspace\\_exo3module_unpacked\\ExchangeOnlineManagement.psd1"
  }
}
```

# Connect-ExchangeOnline Observations

* NOTE that `New-EXOModule` in `ExchangeOnlineManagement.psm1` loads dynamic psd1 and psm1 from the Internet
* DO put a breakpoint on an EXO call, then dive into the newly downloaded `%temp%/tmpEXO_*` folder created by `New-EXOModule`
* DO open `ExchangeOnlineManagement.psm1`, search for `Get-HelpFiles` and comment that line. That loads less stuff.
* DO USE `SkipLoadingFormatData` on Connect-ExchangeOnline. That loads less stuff.
