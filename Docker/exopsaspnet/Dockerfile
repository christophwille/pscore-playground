# Based on https://github.com/dotnet/dotnet-docker/blob/main/src/runtime/6.0/focal/amd64/Dockerfile
# Based on https://github.com/dotnet/dotnet-docker/blob/main/src/aspnet/6.0/focal/amd64/Dockerfile

ARG REPO=exops

# Installer image
FROM amd64/buildpack-deps:focal-curl as installer

# Retrieve .NET
RUN curl -fSL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/6.0.1/dotnet-runtime-6.0.1-linux-x64.tar.gz \
    && dotnet_sha512='2a316e8cba20778b409b8f2a3810348e2805f35afad8aba77a67c4e6bb2c2091e60bc369df22554bb145a5fad0c50e20b39d350b98a85bd33566034a11230da7' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /dotnet \
    && tar -ozxf dotnet.tar.gz -C /dotnet \
    && rm dotnet.tar.gz

# Retrieve ASP.NET Core
RUN curl -fSL --output aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/6.0.1/aspnetcore-runtime-6.0.1-linux-x64.tar.gz \
    && aspnetcore_sha512='9e42c4ac282d3ed099203b9a8a06b4f1baf1267b4d51c9d505ca7127930534b60d4e94022036719133b30c1b503f66d7d4571bc24059d735e510f5e455ec6c51' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -ozxf aspnetcore.tar.gz ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

FROM $REPO:1

# https://github.com/dotnet/dotnet-docker/blob/main/src/runtime-deps/6.0/cbl-mariner1.0/amd64/Dockerfile for additional ENV vars
ENV \
    # Configure web servers to bind to port 80 when present
    ASPNETCORE_URLS=http://+:80 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_VERSION=6.0.1 \
    # ASP.NET Core version
    ASPNET_VERSION=6.0.1 \
    # Set the default console formatter to JSON
    Logging__Console__FormatterName=Json

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]

RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

COPY --from=installer ["/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]