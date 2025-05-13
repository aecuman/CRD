# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5100
ENV ASPNETCORE_URLS=http://+:5100


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install required dependencies to prevent package issues
RUN apt-get update && apt-get install -y libc6 libgcc1 libgssapi-krb5-2 libicu72 libssl3 libstdc++6 libunwind8 zlib1g



# Install .NET SDK explicitly to prevent missing commands
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh && \
    chmod +x dotnet-install.sh && \
    ./dotnet-install.sh --version latest


# Install EF Core tool globally
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"


COPY ["CRD.API/CRD.API.csproj", "CRD.API/"]
COPY ["CRD.Application/CRD.Application.csproj", "CRD.Application/"]
COPY ["CRD.Domain/CRD.Domain.csproj", "CRD.Domain/"]
COPY ["CRD.Infrastructure/CRD.Infrastructure.csproj", "CRD.Infrastructure/"]
COPY ["CRD.Persistence/CRD.Persistence.csproj", "CRD.Persistence/"]
RUN dotnet restore "./CRD.API/CRD.API.csproj"
COPY . .
WORKDIR "/src/CRD.API"
RUN dotnet build "./CRD.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CRD.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Install EF Core tools inside the container
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN apt-get update && apt-get install -y sudo

# Copy entrypoint script for migrations and seeding
COPY CRD.API/entrypoint.sh /app/CRD.API/entrypoint.sh
USER root
RUN sudo chmod +x /app/CRD.API/entrypoint.sh

# Ensure the container runs migrations and seed data before starting the app
ENTRYPOINT ["/app/entrypoint.sh"]


#ENTRYPOINT ["dotnet", "CRD.API.dll"]