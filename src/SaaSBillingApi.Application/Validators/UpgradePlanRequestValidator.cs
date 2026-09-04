using FluentValidation;
using SaaSBillingApi.Application.DTOs;

namespace SaaSBillingApi.Application.Validators;

public class UpgradePlanRequestValidator : AbstractValidator<UpgradePlanRequestDto>
{
    public UpgradePlanRequestValidator()
    {
        RuleFor(x => x.NewPlanId).NotEmpty().WithMessage("NewPlanId is required.");
    }
}