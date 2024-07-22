# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["FinancialDataService.DemoUI/FinancialDataService.DemoUI.csproj", "FinancialDataService.DemoUI/"]
RUN dotnet restore "FinancialDataService.DemoUI/FinancialDataService.DemoUI.csproj"

# Copy remaining files and build the project
COPY . .
WORKDIR "/src/FinancialDataService.DemoUI"
RUN dotnet build "FinancialDataService.DemoUI.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Publish the app
RUN dotnet publish "FinancialDataService.DemoUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 2: Serve the application using ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.DemoUI.dll"]
