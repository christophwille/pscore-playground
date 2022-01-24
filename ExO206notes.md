# ExO 2.0.6-preview 3 Tests

Works on Windows (see [interactive notebook](ExO206tests.ipynb)), works in WSL, works in Docker.
No more remoting module is being created on Connect-ExchangeOnline.

### Interactive in Container

`docker pull mcr.microsoft.com/powershell:7.2.1-ubuntu-20.04`

`docker run -it mcr.microsoft.com/powershell:7.2.1-ubuntu-20.04 /bin/bash`

In container:

```
apt-get update
apt-get -y install sudo

sudo pwsh

# Suppress installation confirmations for PS modules
Set-PSRepository -Name 'PSGallery' -InstallationPolicy Trusted

Install-Module -Name PSWSMan
Install-WSMan

exit

pwsh
Install-Module -Name ExchangeOnlineManagement -RequiredVersion 2.0.6-Preview3 -AllowPrerelease

Connect-ExchangeOnline -Device

Get-EXOMailbox

Disconnect-ExchangeOnline -Confirm:$false
```
