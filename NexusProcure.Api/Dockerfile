# ============================
# 1️⃣ Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files relative to the Api folder
COPY *.csproj ./
COPY ../NexusProcure.Application/*.csproj ../NexusProcure.Application/
COPY ../NexusProcure.Core/*.csproj ../NexusProcure.Core/
COPY ../NexusProcure.Infrastructure/*.csproj ../NexusProcure.Infrastructure/
COPY ../NexusProcure.Shared/*.csproj ../NexusProcure.Shared/

# Restore dependencies
RUN dotnet restore "NexusProcure.Api.csproj"

# Copy the rest of the API code
COPY . .

# Set working directory
WORKDIR /src

# Publish the app
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ============================
# 2️⃣ Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "NexusProcure.Api.dll"]
