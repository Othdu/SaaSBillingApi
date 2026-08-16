using SaaSBillingApi.Application.Interfaces;
using SaaSBillingApi.Domain.Entities;
using SaaSBillingApi.Domain.Services;

namespace SaaSBillingApi.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ProrationService _prorationService;

    public SubscriptionService(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ProrationService prorationService)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _prorationService = prorationService;
    }

    public async Task<Subscription> StartTrialAsync(Guid planId, int trialDays, CancellationToken cancellationToken = default)
    {
        var plan = await _unitOfWork.Plans.GetByIdAsync(planId, cancellationToken);
        if (plan is null)
            throw new KeyNotFoundException($"Plan '{planId}' was not found.");

        var periodStart = DateTime.UtcNow;
        var periodEnd = periodStart.AddDays(trialDays);

        var subscription = new Subscription(_tenantContext.TenantId, planId, periodStart, periodEnd);

        await _unitOfWork.Subscriptions.AddAsync(subscription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subscription;
    }

    public async Task<decimal> UpgradePlanAsync(Guid subscriptionId, Guid newPlanId, CancellationToken cancellationToken = default)
    {
        var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(subscriptionId, cancellationToken);
        if (subscription is null)
            throw new KeyNotFoundException($"Subscription '{subscriptionId}' was not found.");

        var currentPlan = await _unitOfWork.Plans.GetByIdAsync(subscription.PlanId, cancellationToken);
        var newPlan = await _unitOfWork.Plans.GetByIdAsync(newPlanId, cancellationToken);

        if (currentPlan is null)
            throw new KeyNotFoundException($"Current plan '{subscription.PlanId}' was not found.");
        if (newPlan is null)
            throw new KeyNotFoundException($"New plan '{newPlanId}' was not found.");

        var proratedAmount = _prorationService.CalculateProratedAmount(
            currentPlan,
            newPlan,
            subscription.CurrentPeriodStartUtc,
            subscription.CurrentPeriodEndUtc,
            DateTime.UtcNow);

        subscription.ChangePlan(newPlanId);
        _unitOfWork.Subscriptions.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return proratedAmount;
    }

    public async Task CancelAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(subscriptionId, cancellationToken);
        if (subscription is null)
            throw new KeyNotFoundException($"Subscription '{subscriptionId}' was not found.");

        subscription.Cancel();
        _unitOfWork.Subscriptions.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}