FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FinancialDataService.TcpServer/FinancialDataService.TcpServer.csproj", "FinancialDataService.TcpServer/"]
RUN dotnet restore "FinancialDataService.TcpServer/FinancialDataService.TcpServer.csproj"
COPY . .
WORKDIR "/src/FinancialDataService.TcpServer"
RUN dotnet build "FinancialDataService.TcpServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialDataService.TcpServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialDataService.TcpServer.dll"]
