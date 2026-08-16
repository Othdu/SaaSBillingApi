using SaaSBillingApi.Domain.Entities;

namespace SaaSBillingApi.Domain.Services;

public class ProrationService
{
    public decimal CalculateProratedAmount(
        Plan currentPlan,
        Plan newPlan,
        DateTime periodStartUtc,
        DateTime periodEndUtc,
        DateTime changeDateUtc)
    {
        if (currentPlan is null) throw new ArgumentNullException(nameof(currentPlan));
        if (newPlan is null) throw new ArgumentNullException(nameof(newPlan));

        if (periodEndUtc <= periodStartUtc)
            throw new ArgumentException("Period end must be after period start.");

        if (changeDateUtc < periodStartUtc || changeDateUtc > periodEndUtc)
            throw new ArgumentOutOfRangeException(
                nameof(changeDateUtc),
                "Change date must fall within the billing period.");

        var totalDays = (periodEndUtc - periodStartUtc).TotalDays;
        var remainingDays = (periodEndUtc - changeDateUtc).TotalDays;
        var unusedFraction = (decimal)(remainingDays / totalDays);

        var creditForOldPlan = currentPlan.MonthlyPrice * unusedFraction;
        var chargeForNewPlan = newPlan.MonthlyPrice * unusedFraction;

        return Math.Round(chargeForNewPlan - creditForOldPlan, 2, MidpointRounding.AwayFromZero);
    }
}