using Microsoft.AspNetCore.Mvc;
using SaaSBillingApi.Application.Interfaces;

namespace SaaSBillingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly IUnitOfWork _unitofwork;
    public PlansController(IUnitOfWork unitofwork)
    {
        _unitofwork = unitofwork;
    }
    [HttpGet]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var plans = await _unitofwork.Plans.GetAllAsync(cancellationToken);
         
        var Response = plans.Select(p => new
        {
            id = p.Id,
            name = p.Name,
            monthlyPrice = p.MonthlyPrice
        });
         return Ok(Response);

    }
}

