using SaaSBillingApi.Domain.Common;
using SaaSBillingApi.Domain.Enums;
using SaaSBillingApi.Domain.Exceptions;

namespace SaaSBillingApi.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid PlanId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CurrentPeriodStartUtc { get; private set; }
    public DateTime CurrentPeriodEndUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    private Subscription() { } // EF Core

    public Subscription(Guid tenantId, Guid planId, DateTime currentPeriodStartUtc, DateTime currentPeriodEndUtc)
    {
        if (currentPeriodEndUtc <= currentPeriodStartUtc)
            throw new ArgumentException("Period end must be after period start.");

        TenantId = tenantId;
        PlanId = planId;
        Status = SubscriptionStatus.Trial;
        CurrentPeriodStartUtc = currentPeriodStartUtc;
        CurrentPeriodEndUtc = currentPeriodEndUtc;
    }

    public void Activate()
    {
        if (Status != SubscriptionStatus.Trial && Status != SubscriptionStatus.PastDue)
            throw new InvalidSubscriptionTransitionException(Status, SubscriptionStatus.Active);

        Status = SubscriptionStatus.Active;
    }

    public void MarkPastDue()
    {
        if (Status != SubscriptionStatus.Active)
            throw new InvalidSubscriptionTransitionException(Status, SubscriptionStatus.PastDue);

        Status = SubscriptionStatus.PastDue;
    }

    public void Cancel()
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new InvalidSubscriptionTransitionException(Status, SubscriptionStatus.Cancelled);

        Status = SubscriptionStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
    }

    public void RenewPeriod(DateTime newPeriodStartUtc, DateTime newPeriodEndUtc)
    {
        if (Status != SubscriptionStatus.Active)
            throw new InvalidOperationException("Only active subscriptions can be renewed.");

        if (newPeriodEndUtc <= newPeriodStartUtc)
            throw new ArgumentException("Period end must be after period start.");

        CurrentPeriodStartUtc = newPeriodStartUtc;
        CurrentPeriodEndUtc = newPeriodEndUtc;
    }

    public void ChangePlan(Guid newPlanId)
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new InvalidOperationException("Cannot change the plan of a cancelled subscription.");

        PlanId = newPlanId;
    }
}