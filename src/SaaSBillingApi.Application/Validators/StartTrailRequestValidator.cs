using FluentValidation;
using SaaSBillingApi.Application.DTOs;

namespace SaaSBillingApi.Application.Validators;

public class StartTrialRequestValidator : AbstractValidator<StartTrialRequestDto>
{
    public StartTrialRequestValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty().WithMessage("PlanId is required.");
        RuleFor(x => x.TrialDays).InclusiveBetween(1, 90).WithMessage("TrialDays must be between 1 and 90.");
    }
}