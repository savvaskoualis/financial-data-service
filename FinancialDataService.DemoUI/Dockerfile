FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FinancialDataService.DemoUI/FinancialDataService.DemoUI.csproj", "FinancialDataService.DemoUI/"]
COPY ["FinancialDataService.Application/FinancialDataService.Application.csproj", "FinancialDataService.Application/"]
COPY ["FinancialDataService.Domain/FinancialDataService.Domain.csproj", "FinancialDataService.Domain/"]
RUN dotnet restore "FinancialDataService.DemoUI/FinancialDataService.DemoUI.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.DemoUI"
RUN dotnet build "FinancialDataService.DemoUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialDataService.DemoUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.DemoUI.dll"]
