using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TourEgypt.Core.Interfaces.Services;
using TourEgypt.Data.Context;

namespace TourEgypt.Infrastructure.BackgroundJobs
{
    public class PlaceMetricsUpdateJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PlaceMetricsUpdateJob> _logger;

        public PlaceMetricsUpdateJob(IServiceScopeFactory scopeFactory, ILogger<PlaceMetricsUpdateJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Place Metrics Background Job started at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var placeService = scope.ServiceProvider.GetRequiredService<IPlaceService>();
                        await placeService.UpdatePlaceMetricsAsync();
                    }

                    _logger.LogInformation("Place Metrics updated successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while calculating Place Metrics.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}