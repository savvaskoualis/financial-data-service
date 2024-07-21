using FinancialDataService.Application.Interfaces;
using FinancialDataService.Application.Models;
using FinancialDataService.Domain.Entities;
using FinancialDataService.Domain.Interfaces;
using FinancialDataService.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FinancialDataService.Infrastructure.Services
{
    public class LivePricesBackgroundService : BackgroundService
    {
        private readonly ILogger<LivePricesBackgroundService> _logger;
        private readonly IStreamingPriceDataProvider _priceDataProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBackplane _backplane;
        private readonly AsyncQueue<PriceUpdateModel> _updateQueue;
        private Task _processingTask;
        private CancellationTokenSource _cts;
        private ObjectPool<IServiceScope> _scopePool;

        public LivePricesBackgroundService(
            ILogger<LivePricesBackgroundService> logger,
            IStreamingPriceDataProvider priceDataProvider,
            IServiceScopeFactory scopeFactory,
            IBackplane backplane)
        {
            _logger = logger;
            _priceDataProvider = priceDataProvider;
            _scopeFactory = scopeFactory;
            _backplane = backplane;
            _updateQueue = new AsyncQueue<PriceUpdateModel>();

            _priceDataProvider.OnPriceUpdate += OnPriceUpdate;

            var provider = new DefaultObjectPoolProvider();
            _scopePool = provider.Create(new ServiceScopePooledObjectPolicy(_scopeFactory));
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            List<FinancialInstrument> tradingPairs = null;

            using (var scope = _scopeFactory.CreateScope())
            {
                var symbolRepository = scope.ServiceProvider.GetRequiredService<IFinancialInstrumentRepository>();
                do
                {
                    tradingPairs = (await symbolRepository.GetAllAsync()).ToList();
                    if (tradingPairs.Count == 0)
                    {
                        _logger.LogInformation("No trading pairs found, retrying in 10 seconds...");
                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    }
                }
                while (tradingPairs.Count == 0 && !stoppingToken.IsCancellationRequested);

                await _priceDataProvider.StartAsync(tradingPairs.Select(t => t.Symbol).ToList());
                _processingTask = ProcessUpdatesAsync(_cts.Token);
            }
        }

        private void OnPriceUpdate(PriceUpdateModel priceUpdate)
        {
            _updateQueue.Enqueue(priceUpdate);
        }

        private async Task ProcessUpdatesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var priceUpdate = await _updateQueue.DequeueAsync(cancellationToken);

                    using var scope = _scopePool.Get();
                    var symbolRepository = scope.ServiceProvider.GetRequiredService<IFinancialInstrumentRepository>();
                    var symbol = await symbolRepository.GetBySymbolAsync(priceUpdate.Symbol);
                    if (symbol != null)
                    {
                        symbol.LastTradePrice = priceUpdate.Price;
                        symbol.LastTradeQuantity = priceUpdate.Quantity;
                        symbol.LastTradeTime = priceUpdate.TradeTime;
                        await using var context = scope.ServiceProvider.GetRequiredService<FinancialServiceDbContext>();
                        await context.SaveChangesAsync(cancellationToken);

                        var message = $"{symbol.Symbol}|{symbol.LastTradePrice}|{symbol.LastTradeQuantity}|{symbol.LastTradeTime:o}";
                        await _backplane.PublishAsync(message);
                    }

                    priceUpdate = null;
                }
                catch (OperationCanceledException)
                {
                    // Task was canceled
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing price update.");
                }
            }
        }

        public async override Task StopAsync(CancellationToken stoppingToken)
        {
            _cts.Cancel();
            await _priceDataProvider.StopAsync();
            await _processingTask;
            await base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _priceDataProvider.OnPriceUpdate -= OnPriceUpdate;
            _cts?.Dispose();
            base.Dispose();
        }
    }
}
