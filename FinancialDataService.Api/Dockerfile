FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FinancialDataService.Api/FinancialDataService.Api.csproj", "FinancialDataService.Api/"]
COPY ["FinancialDataService.Application/FinancialDataService.Application.csproj", "FinancialDataService.Application/"]
COPY ["FinancialDataService.Domain/FinancialDataService.Domain.csproj", "FinancialDataService.Domain/"]
COPY ["FinancialDataService.Infrastructure/FinancialDataService.Infrastructure.csproj", "FinancialDataService.Infrastructure/"]
RUN dotnet restore "FinancialDataService.Api/FinancialDataService.Api.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.Api"
RUN dotnet build "FinancialDataService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialDataService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.Api.dll"]
