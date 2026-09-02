using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSBillingApi.Application.DTOs
{
    public  class UpgradePlanRequestDto
    { 
        public Guid NewPlanId { get; set; }
    }
}
