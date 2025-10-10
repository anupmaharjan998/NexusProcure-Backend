# ============================
# 1️⃣ Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all csproj files (restore dependencies)
COPY NexusProcure.Api/NexusProcure.Api.csproj NexusProcure.Api/
COPY NexusProcure.Application/NexusProcure.Application.csproj NexusProcure.Application/
COPY NexusProcure.Core/NexusProcure.Core.csproj NexusProcure.Core/
COPY NexusProcure.Infrastructure/NexusProcure.Infrastructure.csproj NexusProcure.Infrastructure/
COPY NexusProcure.Shared/NexusProcure.Shared.csproj NexusProcure.Shared/

# Restore dependencies
RUN dotnet restore NexusProcure.Api/NexusProcure.Api.csproj

# Copy the rest of the source code
COPY . .

# Publish the API
RUN dotnet publish NexusProcure.Api/NexusProcure.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ============================
# 2️⃣ Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Configure port (Render injects PORT environment variable)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Start the API
ENTRYPOINT ["dotnet", "NexusProcure.Api.dll"]
