# Sample loading extracted module

https://www.powershellgallery.com/packages/ExchangeOnlineManagement/3.0.0

Manual Download / Download the raw nupkg file

* Save with extension .zip
* Make sure to go to Properties / check the Unblock and click Apply (otherwise won't execute)
* Extract to folder (you could delete _rels, package, xml + nuspec in root)
* Open \netCore\ExchangeOnlineManagement.psm1. Search for `$ShowBanner` and set it to `$false`
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