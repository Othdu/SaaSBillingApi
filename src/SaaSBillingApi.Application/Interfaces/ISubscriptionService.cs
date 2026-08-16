using SaaSBillingApi.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaaSBillingApi.Application.Interfaces;

public interface ISubscriptionService
{
    Task<Subscription> StartTrialAsync(Guid planId, int trialDays, CancellationToken cancellationToken = default);
    Task<decimal> UpgradePlanAsync(Guid subscriptionId, Guid newPlanId, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
}