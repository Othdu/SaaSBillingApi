using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaaSBillingApi.Domain.Entities;
using SaaSBillingApi.Domain.Enums;

namespace SaaSBillingApi.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Tenants.AnyAsync())
            return; // already seeded

        var tenant = new Tenant("Acme Corp", "acme");
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<User>();
        var passwordHash = hasher.HashPassword(null!, "Password123!");

        var user = new User(tenant.Id, "admin@acme.com", passwordHash, UserRole.TenantAdmin);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}