using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSBillingApi.Application.DTOs
{
    public class StartTrialRequestDto
    {
        public Guid PlanId { get; set; }
        public int TrialDays { get; set; }
    }
}
