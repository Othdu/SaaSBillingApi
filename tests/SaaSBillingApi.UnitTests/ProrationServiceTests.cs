using SaaSBillingApi.Domain.Entities;
using SaaSBillingApi.Domain.Services;
using Xunit;

namespace SaaSBillingApi.UnitTests;

public class ProrationServiceTests
{
    private readonly ProrationService _sut = new();

    [Fact]
    public void UpgradeHalfwayThroughPeriod_ChargesHalfDifference()
    {
        var basicPlan = new Plan("Basic", 10m);
        var proPlan = new Plan("Pro", 30m);

        var periodStart = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var changeDate = new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc);

        var result = _sut.CalculateProratedAmount(basicPlan, proPlan, periodStart, periodEnd, changeDate);

        Assert.Equal(10m, result);
    }

    [Fact]
    public void DowngradeHalfwayThroughPeriod_CreditsCustomer()
    {
        var proPlan = new Plan("Pro", 30m);
        var basicPlan = new Plan("Basic", 10m);

        var periodStart = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var changeDate = new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc);

        var result = _sut.CalculateProratedAmount(proPlan, basicPlan, periodStart, periodEnd, changeDate);

        Assert.Equal(-10m, result);
    }

    [Fact]
    public void ChangeOnPeriodStart_ChargesFullDifference()
    {
        var basicPlan = new Plan("Basic", 10m);
        var proPlan = new Plan("Pro", 30m);

        var periodStart = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        var result = _sut.CalculateProratedAmount(basicPlan, proPlan, periodStart, periodEnd, periodStart);

        Assert.Equal(20m, result);
    }

    [Fact]
    public void ChangeOnPeriodEnd_ChargesNothing()
    {
        var basicPlan = new Plan("Basic", 10m);
        var proPlan = new Plan("Pro", 30m);

        var periodStart = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        var result = _sut.CalculateProratedAmount(basicPlan, proPlan, periodStart, periodEnd, periodEnd);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void ChangeDateOutsidePeriod_Throws()
    {
        var basicPlan = new Plan("Basic", 10m);
        var proPlan = new Plan("Pro", 30m);

        var periodStart = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var invalidChangeDate = periodEnd.AddDays(1);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _sut.CalculateProratedAmount(basicPlan, proPlan, periodStart, periodEnd, invalidChangeDate));
    }
}