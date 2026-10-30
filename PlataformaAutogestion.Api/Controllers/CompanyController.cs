using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Requests;
using PlataformaAutogestion.Domain.Exceptions;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _service.GetAllAsync();
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
                var company = await _service.GetByIdAsync(id);
                return Ok(company);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
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
}
