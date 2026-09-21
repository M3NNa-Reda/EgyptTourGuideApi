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
    public class CityMetricsUpdateJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CityMetricsUpdateJob> _logger;

        public CityMetricsUpdateJob(IServiceScopeFactory scopeFactory, ILogger<CityMetricsUpdateJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("City Metrics Background Job started at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var cityService = scope.ServiceProvider.GetRequiredService<ICityService>();
                        await cityService.UpdateCityMetricsAsync();
                    }

                    _logger.LogInformation("City Metrics updated successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while calculating City Metrics.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}