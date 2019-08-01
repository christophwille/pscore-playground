FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic AS build
WORKDIR /src
COPY ["NetCore.SimpleWebApi/NetCore.SimpleWebApi.csproj", "NetCore.SimpleWebApi/"]
COPY ["NetCore.RemotePS/NetCore.RemotePS.csproj", "NetCore.RemotePS/"]
RUN dotnet restore "NetCore.SimpleWebApi/NetCore.SimpleWebApi.csproj"
COPY . .
WORKDIR "/src/NetCore.SimpleWebApi"
RUN dotnet build "NetCore.SimpleWebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NetCore.SimpleWebApi.csproj" -c Release -o /app

FROM base AS final
RUN apt-get update && \
  apt-get -y install sudo && \
  sudo apt-get install -y software-properties-common && \
  sudo apt-get install wget && \
  wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb && \
  sudo dpkg -i packages-microsoft-prod.deb && \
  sudo apt-get update && \
  sudo add-apt-repository universe && \
  sudo apt-get install -y powershell
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetCore.SimpleWebApi.dll"]