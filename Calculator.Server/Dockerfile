# Use the official .NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Calculator.Server/Calculator.Server.csproj", "Calculator.Server/"]
COPY ["Calculator.Shared/Calculator.Shared.csproj", "Calculator.Shared/"]

# Restore dependencies
RUN dotnet restore "Calculator.Server/Calculator.Server.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Calculator.Server"
RUN dotnet build "Calculator.Server.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Calculator.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Calculator.Server.dll"]