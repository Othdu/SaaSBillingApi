using System;

namespace SaaSBillingApi.Application.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
}
