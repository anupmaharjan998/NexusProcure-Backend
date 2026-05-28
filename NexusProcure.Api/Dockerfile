# ============================
# Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for better Docker layer caching
COPY NexusProcure.Api/NexusProcure.Api.csproj NexusProcure.Api/
COPY NexusProcure.Application/NexusProcure.Application.csproj NexusProcure.Application/
COPY NexusProcure.Core/NexusProcure.Core.csproj NexusProcure.Core/
COPY NexusProcure.Infrastructure/NexusProcure.Infrastructure.csproj NexusProcure.Infrastructure/
COPY NexusProcure.Shared/NexusProcure.Shared.csproj NexusProcure.Shared/

RUN dotnet restore NexusProcure.Api/NexusProcure.Api.csproj

# Copy source code
COPY . .

# Publish the API
RUN dotnet publish NexusProcure.Api/NexusProcure.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================
# Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render provides PORT at runtime. Default to 10000, Render's default web-service port.
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=10000
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

EXPOSE 10000

ENTRYPOINT ["dotnet", "NexusProcure.Api.dll"]