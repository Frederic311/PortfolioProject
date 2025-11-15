# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["PortfolioApp.API/PortfolioApp.API.csproj", "PortfolioApp.API/"]
COPY ["PortfolioApp.Shared/PortfolioApp.Shared.csproj", "PortfolioApp.Shared/"]

# Restore dependencies
RUN dotnet restore "PortfolioApp.API/PortfolioApp.API.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/PortfolioApp.API"
RUN dotnet build "PortfolioApp.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PortfolioApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PortfolioApp.API.dll"]
