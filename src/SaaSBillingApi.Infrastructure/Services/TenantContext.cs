using Microsoft.AspNetCore.Http;
using SaaSBillingApi.Application.Interfaces;

namespace SaaSBillingApi.Infrastructure.Services;

public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");

            if (tenantIdClaim is null || !Guid.TryParse(tenantIdClaim.Value, out var tenantId))
                throw new InvalidOperationException("No tenant context is available for the current request.");

            return tenantId;
        }
    }
}