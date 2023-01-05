#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExO3onNet7Containerized/ExO3onNet7Containerized.csproj", "ExO3onNet7Containerized/"]
COPY ["ExO3PsLib/ExO3PsLib.csproj", "ExO3PsLib/"]
RUN dotnet restore "ExO3onNet7Containerized/ExO3onNet7Containerized.csproj"
COPY . .
WORKDIR "/src/ExO3onNet7Containerized"
RUN dotnet build "ExO3onNet7Containerized.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExO3onNet7Containerized.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExO3onNet7Containerized.dll"]