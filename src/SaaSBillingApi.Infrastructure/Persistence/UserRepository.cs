using Microsoft.EntityFrameworkCore;
using SaaSBillingApi.Application.Interfaces;
using SaaSBillingApi.Domain.Entities;

namespace SaaSBillingApi.Infrastructure.Persistence;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower(), cancellationToken);
    }
}