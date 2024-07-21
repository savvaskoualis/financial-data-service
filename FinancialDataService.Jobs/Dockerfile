FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinancialDataService.Jobs/FinancialDataService.Jobs.csproj", "FinancialDataService.Jobs/"]
RUN dotnet restore "FinancialDataService.Jobs/FinancialDataService.Jobs.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.Jobs"
RUN dotnet build "FinancialDataService.Jobs.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FinancialDataService.Jobs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.Jobs.dll"]
