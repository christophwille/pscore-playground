#  ExO and PowerShell 7.2

Docs landing page
https://docs.microsoft.com/en-us/powershell/exchange/exchange-online-powershell

Installation
https://docs.microsoft.com/en-us/powershell/exchange/exchange-online-powershell-v2?view=exchange-ps#install-and-maintain-the-exo-v2-module

## Windows:

```powershell
Import-Module ExchangeOnlineManagement
Connect-ExchangeOnline
Get-EXOMailbox
```

Will open browser windows for login by default.

## Linux

https://docs.microsoft.com/en-us/powershell/exchange/exchange-online-powershell-v2?view=exchange-ps#install-and-maintain-the-exo-v2-module

Quote: "The EXO V2 module is officially supported in the following distributions of Linux:

* Ubuntu 18.04 LTS
* Ubuntu 20.04 LTS"

```
sudo pwsh

Install-Module -Name PSWSMan
Install-WSMan
```

Section "Prerequisites for the EXO V2 module" not applicable it seems:

```powershell
Set-ExecutionPolicy RemoteSigned
# --> Set-ExecutionPolicy: Operation is not supported on this platform.
```

Test:

```powershell
# don't do this in 'sudo pwsh'
Install-Module -Name ExchangeOnlineManagement

Connect-ExchangeOnline -Device
# takes some time, including Creating implicit remoting module

Get-EXOMailbox
```

Do not forget to disconnect:

```powershell
Disconnect-ExchangeOnline
# Running this cmdlet clears all active sessions created using Connect-ExchangeOnline or Connect-IPPSSession.
# Press(Y/y/A/a) if you want to continue.
```


## App-only authentication

Doc and steps https://docs.microsoft.com/en-us/powershell/exchange/app-only-auth-powershell-v2

Best to simply follow the steps to register the app.

https://github.com/dgoldman-msft/PSServicePrincipal/ no longer useful IMO (Windows PowerShell, other issues)


## Docker

https://docs.microsoft.com/en-us/dotnet/architecture/microservices/net-core-net-framework-containers/net-container-os-targets

* https://github.com/dotnet/dotnet-docker/blob/main/src/runtime/6.0/focal/amd64/Dockerfile
* https://github.com/dotnet/dotnet-docker/blob/main/src/aspnet/6.0/focal/amd64/Dockerfile

List of all tags for PowerShell containers
https://hub.docker.com/_/microsoft-powershell - 7.2.0-ubuntu-focal-20211102
https://mcr.microsoft.com/v2/powershell/tags/list

Supported parent images for Windows App Service containers
https://docs.microsoft.com/en-us/azure/app-service/configure-custom-container?pivots=container-windows#supported-parent-images