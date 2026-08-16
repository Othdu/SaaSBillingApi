using SaaSBillingApi.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSBillingApi.Domain.Entities
{
    public class Plan : BaseEntity
    {
        public string Name   { get; private set; } = null!;
        public decimal MonthlyPrice { get; private set; }
        public bool IsActive   { get; private set; }

        private Plan() { }
        public Plan (string name, decimal monthlyPrice)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Plan name cannot be null or empty.", nameof(name));
            if (monthlyPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(monthlyPrice), "Monthly price cannot be negative.");
            Name = name;
            MonthlyPrice = monthlyPrice;
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }

    }
}
