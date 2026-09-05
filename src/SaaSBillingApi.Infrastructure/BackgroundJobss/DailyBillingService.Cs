using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SaaSBillingApi.Domain.Enums;
using SaaSBillingApi.Infrastructure.Persistence;
using System.Threading;

namespace SaaSBillingApi.Infrastructure.BackgroundJobs;

public class DailyBillingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopefactory;
    private readonly ILogger<DailyBillingService> _logger;
    private static readonly TimeSpan interval = TimeSpan.FromHours(24);

    public DailyBillingService(IServiceScopeFactory scopefactory, ILogger<DailyBillingService> logger)
    {
        _scopefactory = scopefactory;
        _logger = logger;
    }
     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessSubscriptionsAsync(stoppingToken);
            await Task.Delay(interval, stoppingToken);
        }
    }
    private async Task ProcessSubscriptionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopefactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var subscriptions = await context.Subscriptions
            .IgnoreQueryFilters()
            .Where(s => s.CurrentPeriodEndUtc <= now &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
            .ToListAsync(cancellationToken);
        foreach (var subscription in subscriptions)
        {
            try
            {
                if (subscription.Status == SubscriptionStatus.Active)
                {
                    subscription.MarkPastDue();
                    _logger.LogInformation($"Subscription {subscription.Id} for Tenant {subscription.TenantId} marked as Past Due.");
                }
                else if (subscription.Status == SubscriptionStatus.Trial)
                {
                    subscription.Cancel();
                    _logger.LogInformation($"Subscription {subscription.Id} for Tenant {subscription.TenantId} marked as Cancelled.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing subscription {subscription.Id} for Tenant {subscription.TenantId}.");
            }

        }
        if (subscriptions.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
        }

    }   

}