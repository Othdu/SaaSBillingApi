// src/SaaSBillingApi.Application/Interfaces/IUnitOfWork.cs
using SaaSBillingApi.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SaaSBillingApi.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<Tenant> Tenants { get; }
    IRepository<Plan> Plans { get; }
    IRepository<Subscription> Subscriptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}