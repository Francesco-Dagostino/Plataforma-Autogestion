using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Infrastructure.Data;
using System.Security.Claims;

namespace PlataformaAutogestion.Api.Middleware;

public sealed class ActiveCompanyMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveCompanyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var principal = context.User;
        var companyClaim = principal.FindFirst("IdCompany")?.Value;

        if (principal.Identity?.IsAuthenticated == true
            && !principal.IsInRole("SuperAdmin")
            && int.TryParse(companyClaim, out var companyId))
        {
            var historyOnlyToken = principal.FindFirst("HistoryOnly")?.Value == "true";
            var companyState = await dbContext.Companys.IgnoreQueryFilters().AsNoTracking()
                .Where(company => company.Id == companyId)
                .Select(company => new { company.IsActive, company.HistoryAccessEnabled })
                .FirstOrDefaultAsync();

            if (companyState == null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "La empresa asociada a la sesión ya no existe." });
                return;
            }

            if (!companyState.IsActive
                && (!companyState.HistoryAccessEnabled || !principal.IsInRole("Admin") || !historyOnlyToken))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "La empresa está desactivada. Para consultar el historial, cerrá sesión e ingresá nuevamente con el permiso habilitado." });
                return;
            }

            if (historyOnlyToken && companyState.IsActive)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "La empresa fue reactivada. Cerrá sesión e ingresá nuevamente para recuperar el acceso normal." });
                return;
            }

            if (historyOnlyToken && context.Request.Method is not ("GET" or "HEAD" or "OPTIONS"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "El acceso habilitado es solo de consulta. No se permiten modificaciones." });
                return;
            }
        }

        await _next(context);
    }
}
