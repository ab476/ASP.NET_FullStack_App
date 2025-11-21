namespace AuthAPI.Modules.Auth.Middleware;

public interface ITenantResolver
{
    string? CurrentTenant { get; }
    void SetTenant(string tenantId);
}

public class TenantResolver : ITenantResolver
{
    private string? _tenant;
    public string? CurrentTenant => _tenant;
    public void SetTenant(string tenantId) => _tenant = tenantId;
}

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    public TenantMiddleware(RequestDelegate next) { _next = next; }

    public async Task Invoke(HttpContext ctx, ITenantResolver resolver)
    {
        // Example: tenant in header X-Tenant-ID or subdomain or JWT claim
        if (ctx.Request.Headers.TryGetValue("X-Tenant-ID", out var t)) resolver.SetTenant(t.First());
        else if (ctx.User?.Identity?.IsAuthenticated == true)
        {
            var tid = ctx.User.FindFirst("tenantId")?.Value;
            if (!string.IsNullOrEmpty(tid)) resolver.SetTenant(tid);
        }

        await _next(ctx);
    }
}
