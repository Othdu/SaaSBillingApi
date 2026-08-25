using SaaSBillingApi.Domain.Entities;

namespace SaaSBillingApi.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}