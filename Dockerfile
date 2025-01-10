# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Install cron
RUN apt-get update && apt-get install -y curl
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Webapi/Webapi.csproj", "Webapi/"]
RUN dotnet restore "./Webapi/Webapi.csproj"
COPY . .
WORKDIR "/src/Webapi"
RUN dotnet build "./Webapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Webapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY /CertKeys/Webapi.pfx /CertKeys/Webapi.pfx

# Copy certificate and published app
COPY --from=publish /app/publish .
# ASP.NET Core environment variables for SSL
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/CertKeys/Webapi.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="YourSecretHere"

USER root


ENTRYPOINT ["dotnet", "Webapi.dll"]