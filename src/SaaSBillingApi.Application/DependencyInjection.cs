using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SaaSBillingApi.Application.Interfaces;
using SaaSBillingApi.Application.Services;
using SaaSBillingApi.Application.Validators;
using SaaSBillingApi.Domain.Services;

namespace SaaSBillingApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ProrationService>();
        services.AddValidatorsFromAssemblyContaining<StartTrialRequestValidator>();

        return services;
    }
}