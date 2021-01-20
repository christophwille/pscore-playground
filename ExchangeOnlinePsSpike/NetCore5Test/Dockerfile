#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

# https://hub.docker.com/_/microsoft-dotnet-aspnet/
FROM cwi/d365exobase AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# https://hub.docker.com/_/microsoft-dotnet-sdk/
FROM mcr.microsoft.com/dotnet/sdk:5.0.100-rc.2-nanoserver-2004 AS build
WORKDIR /src
COPY ["NetCore5Test/NetCore5Test.csproj", "NetCore5Test/"]
RUN dotnet restore "NetCore5Test/NetCore5Test.csproj"
COPY . .
WORKDIR "/src/NetCore5Test"
RUN dotnet build "NetCore5Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetCore5Test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetCore5Test.dll"]