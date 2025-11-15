# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["PortfolioApp.Client/PortfolioApp.Client.csproj", "PortfolioApp.Client/"]
COPY ["PortfolioApp.Shared/PortfolioApp.Shared.csproj", "PortfolioApp.Shared/"]

# Restore dependencies
RUN dotnet restore "PortfolioApp.Client/PortfolioApp.Client.csproj"

# Copy all source code
COPY . .

# Build and publish Blazor WASM
WORKDIR "/src/PortfolioApp.Client"
RUN dotnet publish "PortfolioApp.Client.csproj" -c Release -o /app/publish

# Runtime stage (nginx pour servir le Blazor WASM)
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Copy Blazor WASM files
COPY --from=build /app/publish/wwwroot .

# Copy nginx configuration
COPY PortfolioApp.Client/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
