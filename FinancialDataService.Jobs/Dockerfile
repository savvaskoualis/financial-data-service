FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FinancialDataService.Jobs/FinancialDataService.Jobs.csproj", "FinancialDataService.Jobs/"]
COPY ["FinancialDataService.Application/FinancialDataService.Application.csproj", "FinancialDataService.Application/"]
COPY ["FinancialDataService.Domain/FinancialDataService.Domain.csproj", "FinancialDataService.Domain/"]
COPY ["FinancialDataService.Infrastructure/FinancialDataService.Infrastructure.csproj", "FinancialDataService.Infrastructure/"]
RUN dotnet restore "FinancialDataService.Jobs/FinancialDataService.Jobs.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.Jobs"
RUN dotnet build "FinancialDataService.Jobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialDataService.Jobs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.Jobs.dll"]
