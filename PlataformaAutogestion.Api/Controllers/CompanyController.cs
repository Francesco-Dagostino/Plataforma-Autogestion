using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Requests;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Infrastructure.Data;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;
        private readonly ApplicationDbContext _context;

        public CompanyController(ICompanyService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _context.Companys.IgnoreQueryFilters().AsNoTracking()
                .OrderBy(company => company.Name)
                .Select(company => new
                {
                    company.Id,
                    company.Name,
                    company.Cuit,
                    company.DateHigh,
                    company.IsActive,
                    company.HistoryAccessEnabled
                })
                .ToListAsync();
            return Ok(companies);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("mi-empresa")]
        public async Task<IActionResult> GetMine()
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            var company = await _service.GetByIdAsync(companyId);
            return Ok(company);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("mi-empresa")]
        public async Task<IActionResult> UpdateMine([FromBody] CompanyProfileRequest dto)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            await _service.UpdateProfileAsync(companyId, dto);
            return NoContent();
        }

        // El Admin lo configura y el Empleado lo consulta para saber cuántas horas puede declarar por día.
        [Authorize(Roles = "Admin, Empleado")]
        [HttpGet("tope-horas")]
        public async Task<IActionResult> GetDailyLimit()
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            var company = await _service.GetByIdAsync(companyId);
            return Ok(new { dailyHoursLimit = company.DailyHoursLimit });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("tope-horas")]
        public async Task<IActionResult> UpdateDailyLimit([FromBody] CompanyDailyLimitRequest dto)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            await _service.UpdateDailyLimitAsync(companyId, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("valor-hora-feriado")]
        public async Task<IActionResult> UpdateHolidayRate([FromBody] CompanyHolidayRateRequest dto)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            await _service.UpdateHolidayRateAsync(companyId, dto, CurrentUserId, User.Identity?.Name);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("valor-hora")]
        public async Task<IActionResult> UpdateHourlyRate([FromBody] CompanyHourlyRateRequest dto)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            await _service.UpdateHourlyRateAsync(companyId, dto, CurrentUserId, User.Identity?.Name);
            return NoContent();
        }

        // Cambios de valor hora (normal y de feriado) de la empresa, del más nuevo al más viejo.
        [Authorize(Roles = "Admin")]
        [HttpGet("historial-valores")]
        public async Task<IActionResult> GetRateHistory()
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            return Ok(await _service.GetRateHistoryAsync(companyId));
        }

        private int? CurrentUserId =>
            int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyCreateRequest dto)
        {
            var company = await _service.CreateAsync(dto);
            return Ok(company);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var company = await _context.Companys.IgnoreQueryFilters().AsNoTracking()
                    .Where(c => c.Id == id)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Cuit,
                        c.DateHigh,
                        c.IsActive,
                        c.HistoryAccessEnabled
                    })
                    .FirstOrDefaultAsync();
                if (company == null) return NotFound("Empresa no encontrada.");
                return Ok(company);
            }
            catch (EntityNotFoundException ex) { return NotFound(ex.Message); }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("{id:int}/historial")]
        public async Task<IActionResult> GetHistory(int id)
        {
            return await GetSummary(id);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("{id:int}/resumen")]
        public async Task<IActionResult> GetSummary(int id)
        {
            var company = await _context.Companys.IgnoreQueryFilters().AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new { c.Id, c.Name, c.IsActive, c.HistoryAccessEnabled })
                .FirstOrDefaultAsync();
            if (company == null) return NotFound("Empresa no encontrada.");

            var users = _context.Users.IgnoreQueryFilters().Where(u => u.IdCompany == id);
            var workdays = _context.Workdays.IgnoreQueryFilters().Where(w => w.IdCompany == id);
            var liquidations = _context.Liquidations.IgnoreQueryFilters().Where(l => l.IdCompany == id);
            var details = _context.Details.IgnoreQueryFilters().Where(d => d.IdCompany == id);
            var notifications = _context.Notifications.IgnoreQueryFilters().Where(n => n.IdCompany == id);

            return Ok(new
            {
                company,
                counts = new
                {
                    employees = await users.CountAsync(u => u.role == Roles.Empleado),
                    users = await users.CountAsync(),
                    workdays = await workdays.CountAsync(),
                    liquidations = await liquidations.CountAsync(),
                    liquidationDetails = await details.CountAsync(),
                    notifications = await notifications.CountAsync()
                }
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("consulta-historial")]
        public async Task<IActionResult> GetAdminHistoricalConsultation()
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
            var company = await _context.Companys.IgnoreQueryFilters().AsNoTracking()
                .Where(c => c.Id == companyId)
                .Select(c => new { c.Id, c.Name, c.IsActive })
                .FirstOrDefaultAsync();
            if (company == null) return NotFound("Empresa no encontrada.");

            var users = await _context.Users.IgnoreQueryFilters().AsNoTracking()
                .Where(u => u.IdCompany == companyId)
                .OrderBy(u => u.Name)
                .Select(u => new { u.Id, u.Name, u.UserName, Role = u.role.ToString(), u.IsActive })
                .ToListAsync();
            var workdays = await _context.Workdays.IgnoreQueryFilters().AsNoTracking()
                .Where(w => w.IdCompany == companyId)
                .OrderByDescending(w => w.DateEntry)
                .Select(w => new { w.Id, w.DateEntry, w.HoursWorked, State = w.Estado.ToString(), Employee = _context.Users.IgnoreQueryFilters().Where(u => u.Id == w.IdUser).Select(u => u.Name).FirstOrDefault(), w.MotivoRechazo })
                .ToListAsync();
            var liquidations = await _context.Liquidations.IgnoreQueryFilters().AsNoTracking()
                .Where(l => l.IdCompany == companyId)
                .OrderByDescending(l => l.LiquidationDate)
                .Select(l => new { l.Id, l.LiquidationDate, l.ExecutionDate, l.Total, l.IsClosed })
                .ToListAsync();
            var details = await _context.Details.IgnoreQueryFilters().AsNoTracking()
                .Where(d => d.IdCompany == companyId)
                .OrderByDescending(d => d.Liquidation.LiquidationDate)
                .Select(d => new { d.Id, d.IdLiquidation, Period = d.Liquidation.LiquidationDate, Employee = _context.Users.IgnoreQueryFilters().Where(u => u.Id == d.IdUser).Select(u => u.Name).FirstOrDefault(), d.TotalHours, d.Amount })
                .ToListAsync();
            var rateHistory = await _context.RateHistories.IgnoreQueryFilters().AsNoTracking()
                .Where(r => r.IdCompany == companyId)
                .OrderByDescending(r => r.ChangedAt)
                .Select(r => new { r.Id, r.RateType, r.Value, r.PreviousValue, r.ChangedAt, r.UserName })
                .ToListAsync();
            var notifications = await _context.Notifications.IgnoreQueryFilters().AsNoTracking()
                .Where(n => n.IdCompany == companyId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new { n.Id, n.Type, n.Message, n.CreatedAt, n.IsRead })
                .ToListAsync();

            return Ok(new { company, users, workdays, liquidations, details, rateHistory, notifications });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> SetActive(int id, [FromBody] CompanyActiveRequest request)
        {
            try
            {
                await _service.SetActiveAsync(id, request.IsActive);
                return NoContent();
            }
            catch (EntityNotFoundException ex) { return NotFound(ex.Message); }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id:int}/acceso-historial")]
        public async Task<IActionResult> SetHistoryAccess(int id, [FromBody] HistoryAccessRequest request)
        {
            try
            {
                await _service.SetHistoryAccessAsync(id, request.Enabled);
                return NoContent();
            }
            catch (EntityNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CompanyCreateRequest dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    public class CompanyActiveRequest
    {
        public bool IsActive { get; set; }
    }

    public class HistoryAccessRequest
    {
        public bool Enabled { get; set; }
    }
}
