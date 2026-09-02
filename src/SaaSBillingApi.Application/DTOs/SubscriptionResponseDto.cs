using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSBillingApi.Application.DTOs
{
    public class SubscriptionResponseDto
    {
        public Guid Id { get; set; }
        public Guid PlanId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CurrentPeriodStartUtc { get; set; }
        public DateTime CurrentPeriodEndUtc{ get; set; }

    }
}
