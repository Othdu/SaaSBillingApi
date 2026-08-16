using SaaSBillingApi.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSBillingApi.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public DateTime CreatedAtUtc { get; private set; }

        private Tenant() { }
        public Tenant(string name, string slug)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tenant name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Tenant slug cannot be null or empty.", nameof(slug));
            Name = name;
            Slug = slug;
            CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
