using FluentValidation;
using SaaSBillingApi.Application.DTOs;

namespace SaaSBillingApi.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}