using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSBillingApi.Application.DTOs;
using SaaSBillingApi.Application.Interfaces;
using SaaSBillingApi.Domain.Exceptions;

namespace SaaSBillingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "TenantAdmin")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost("trial")]
    public async Task<IActionResult> StartTrial(StartTrialRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await _subscriptionService.StartTrialAsync(request.PlanId, request.TrialDays, cancellationToken);

            var response = new SubscriptionResponseDto
            {
                Id = subscription.Id,
                PlanId = subscription.PlanId,
                Status = subscription.Status.ToString(),
                CurrentPeriodStartUtc = subscription.CurrentPeriodStartUtc,
                CurrentPeriodEndUtc = subscription.CurrentPeriodEndUtc
            };

            return CreatedAtAction(nameof(StartTrial), new { id = response.Id }, response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/upgrade")]
    public async Task<IActionResult> UpgradePlan(Guid id, UpgradePlanRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var proratedAmount = await _subscriptionService.UpgradePlanAsync(id, request.NewPlanId, cancellationToken);
            return Ok(new { proratedAmount });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _subscriptionService.CancelAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}