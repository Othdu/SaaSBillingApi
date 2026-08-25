using SaaSBillingApi.Domain.Common;
using SaaSBillingApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSBillingApi.Domain.Entities
{
    public class User :BaseEntity
    {
      public Guid TenantId { get; private set; }

        public string Email { get; set; }
        public string PasswordHash { get; set; }
       public UserRole Role { get; set; }
        private User() { }
        public User(Guid tenantId, string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));
            }
            TenantId = tenantId;
            Email = email.ToLowerInvariant(); 
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
