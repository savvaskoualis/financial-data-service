FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinancialDataService.Streams/FinancialDataService.Streams.csproj", "FinancialDataService.Streams/"]
RUN dotnet restore "FinancialDataService.Streams/FinancialDataService.Streams.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.WebSockets"
RUN dotnet build "FinancialDataService.Streams.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FinancialDataService.Streams.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.Streams.dll"]
