using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using System;
using QuantityMeasurementApp.Repository.Sync;

namespace QuantityMeasurementApp.Repository
{
    public class PendingSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PendingSyncBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var syncRepo = scope.ServiceProvider.GetRequiredService<IQuantityMeasurementRepository>() as QuantityMeasurementSyncRepository;
                    if (syncRepo != null)
                    {
                        syncRepo.SyncPendingToDatabase();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PendingSyncBackgroundService] Error during sync: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
