FROM mcr.microsoft.com/powershell:7.2.1-ubuntu-20.04

RUN apt-get update && \
      apt-get -y install sudo

RUN sudo pwsh \
        -NoLogo \
        -NoProfile \
        -Command "Set-PSRepository -Name 'PSGallery' -InstallationPolicy Trusted; \
          Install-Module -Name PSWSMan; \
          Install-WSMan"

RUN pwsh \
        -NoLogo \
        -NoProfile \
        -Command "Install-Module -Name ExchangeOnlineManagement -RequiredVersion 2.0.6-Preview3 -AllowPrerelease"