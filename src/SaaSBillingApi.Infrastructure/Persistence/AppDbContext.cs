using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaSBillingApi.Application.Interfaces;
using SaaSBillingApi.Domain.Entities;

namespace SaaSBillingApi.Infrastructure.Persistence
{
    public class AppDbContext :DbContext
    {
        private readonly ITenantContext _tenantContext;

        public DbSet<Tenant>Tenants =>Set<Tenant>();
        public DbSet<Plan> Plans => Set<Plan>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : base(options)
        {
            _tenantContext = tenantContext;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure Tenant entity
            modelBuilder.Entity<Tenant>(entity =>
            {
              
                entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Slug).IsRequired().HasMaxLength(100);
                entity.HasIndex(t => t.Slug).IsUnique();
            });
            // Configure Plan entity
            modelBuilder.Entity<Plan>(entity =>
            {
               
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.MonthlyPrice).IsRequired().HasColumnType("decimal(18,2)");
            });
            // Configure Subscription entity
            modelBuilder.Entity<Subscription>(entity =>
            {

                // Define relationships
                entity.HasOne<Tenant>()
                      .WithMany()
                      .HasForeignKey(s => s.TenantId);
                entity.HasOne<Plan>()
                      .WithMany()
                      .HasForeignKey(s => s.PlanId);
            });

        }
    }

    
}
