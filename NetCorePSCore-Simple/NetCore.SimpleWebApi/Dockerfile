FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
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
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetCore.SimpleWebApi.dll"]