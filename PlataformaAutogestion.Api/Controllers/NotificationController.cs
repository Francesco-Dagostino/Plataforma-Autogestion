using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int CompanyId => int.Parse(User.FindFirst("IdCompany")!.Value);

        // Últimos avisos de la empresa y cuántos siguen sin leer.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _notificationService.GetByCompanyAsync(CompanyId));
        }

        [HttpPut("{id:int}/leer")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(CompanyId, id);
            return NoContent();
        }

        [HttpPut("leer-todas")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(CompanyId);
            return NoContent();
        }
    }
}
