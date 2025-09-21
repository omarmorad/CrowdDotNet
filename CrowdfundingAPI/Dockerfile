# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/CrowdfundingAPI.API/CrowdfundingAPI.API.csproj", "src/CrowdfundingAPI.API/"]
COPY ["src/CrowdfundingAPI.Application/CrowdfundingAPI.Application.csproj", "src/CrowdfundingAPI.Application/"]
COPY ["src/CrowdfundingAPI.Domain/CrowdfundingAPI.Domain.csproj", "src/CrowdfundingAPI.Domain/"]
COPY ["src/CrowdfundingAPI.Infrastructure/CrowdfundingAPI.Infrastructure.csproj", "src/CrowdfundingAPI.Infrastructure/"]

RUN dotnet restore "src/CrowdfundingAPI.API/CrowdfundingAPI.API.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/CrowdfundingAPI.API"
RUN dotnet build "CrowdfundingAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CrowdfundingAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CrowdfundingAPI.API.dll"]