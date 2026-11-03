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
            var isActive = await dbContext.Companys.IgnoreQueryFilters().AsNoTracking()
                .AnyAsync(company => company.Id == companyId && company.IsActive);

            if (!isActive)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "La empresa está desactivada. Contactá al administrador de la plataforma." });
                return;
            }
        }

        await _next(context);
    }
}
