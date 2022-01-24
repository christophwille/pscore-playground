#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Docker for Windows: Settings -> Docker Engine -> buildkit = false on "failed to solve with frontend dockerfile.v0: failed to read dockerfile:"

FROM exopsaspnet:2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExOonNet6/ExOonNet6.csproj", "ExOonNet6/"]
RUN dotnet restore "ExOonNet6/ExOonNet6.csproj"
COPY . .
WORKDIR "/src/ExOonNet6"
RUN dotnet build "ExOonNet6.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExOonNet6.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExOonNet6.dll"]