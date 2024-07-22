FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FinancialDataService.Streams/FinancialDataService.Streams.csproj", "FinancialDataService.Streams/"]
COPY ["FinancialDataService.Application/FinancialDataService.Application.csproj", "FinancialDataService.Application/"]
COPY ["FinancialDataService.Domain/FinancialDataService.Domain.csproj", "FinancialDataService.Domain/"]
COPY ["FinancialDataService.Infrastructure/FinancialDataService.Infrastructure.csproj", "FinancialDataService.Infrastructure/"]
RUN dotnet restore "FinancialDataService.Streams/FinancialDataService.Streams.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.Streams"
RUN dotnet build "FinancialDataService.Streams.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialDataService.Streams.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.Streams.dll"]
