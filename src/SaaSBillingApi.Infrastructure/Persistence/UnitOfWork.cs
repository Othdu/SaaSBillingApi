using SaaSBillingApi.Application.Interfaces;
using SaaSBillingApi.Domain.Entities;

namespace SaaSBillingApi.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<Tenant> Tenants { get; }
    public IRepository<Plan> Plans { get; }
    public IRepository<Subscription> Subscriptions { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Tenants = new Repository<Tenant>(context);
        Plans = new Repository<Plan>(context);
        Subscriptions = new Repository<Subscription>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}