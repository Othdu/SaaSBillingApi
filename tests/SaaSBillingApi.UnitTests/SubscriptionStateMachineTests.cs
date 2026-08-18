using SaaSBillingApi.Domain.Entities;
using SaaSBillingApi.Domain.Enums;
using SaaSBillingApi.Domain.Exceptions;
using Xunit;

namespace SaaSBillingApi.UnitTests;

public class SubscriptionStateMachineTests
{
    private static Subscription CreateTrialSubscription()
    {
        return new Subscription(
            tenantId: Guid.NewGuid(),
            planId: Guid.NewGuid(),
            currentPeriodStartUtc: DateTime.UtcNow,
            currentPeriodEndUtc: DateTime.UtcNow.AddDays(14));
    }

    [Fact]
    public void NewSubscription_StartsInTrial()
    {
        var subscription = CreateTrialSubscription();

        Assert.Equal(SubscriptionStatus.Trial, subscription.Status);
    }

    [Fact]
    public void Activate_FromTrial_MovesToActive()
    {
        var subscription = CreateTrialSubscription();

        subscription.Activate();

        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
    }

    [Fact]
    public void Activate_FromPastDue_MovesToActive()
    {
        var subscription = CreateTrialSubscription();
        subscription.Activate();
        subscription.MarkPastDue();

        subscription.Activate();

        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
    }

    [Fact]
    public void Activate_FromCancelled_Throws()
    {
        var subscription = CreateTrialSubscription();
        subscription.Cancel();

        Assert.Throws<InvalidSubscriptionTransitionException>(() => subscription.Activate());
    }

    [Fact]
    public void MarkPastDue_FromTrial_Throws()
    {
        var subscription = CreateTrialSubscription();

        Assert.Throws<InvalidSubscriptionTransitionException>(() => subscription.MarkPastDue());
    }

    [Fact]
    public void MarkPastDue_FromActive_MovesToPastDue()
    {
        var subscription = CreateTrialSubscription();
        subscription.Activate();

        subscription.MarkPastDue();

        Assert.Equal(SubscriptionStatus.PastDue, subscription.Status);
    }

    [Fact]
    public void Cancel_FromAnyNonCancelledState_MovesToCancelled()
    {
        var subscription = CreateTrialSubscription();

        subscription.Cancel();

        Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
        Assert.NotNull(subscription.CancelledAtUtc);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_Throws()
    {
        var subscription = CreateTrialSubscription();
        subscription.Cancel();

        Assert.Throws<InvalidSubscriptionTransitionException>(() => subscription.Cancel());
    }

    [Fact]
    public void ChangePlan_WhenCancelled_Throws()
    {
        var subscription = CreateTrialSubscription();
        subscription.Cancel();

        Assert.Throws<InvalidOperationException>(() => subscription.ChangePlan(Guid.NewGuid()));
    }
}