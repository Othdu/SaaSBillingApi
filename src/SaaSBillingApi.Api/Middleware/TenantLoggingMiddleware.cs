using Serilog.Context; 

namespace SaaSBillingApi.Api.Middleware
{
    public class TenantLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var tenantId = context.User?.FindFirst("tenant_id")?.Value ?? "NoTenant";
            using (LogContext.PushProperty("TenantId", tenantId))
            {
                await _next(context);
            }
        }
    }
}
