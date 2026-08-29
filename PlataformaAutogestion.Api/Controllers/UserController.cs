using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Exceptions;
using System.Security.Claims;

namespace PlataformaAutogestion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        

        [Authorize(Roles = "Admin, SuperAdmin")] 
        [HttpGet("Mi Empresa")]
        public async Task<IActionResult> GetAll()
        {
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                // Admin solo ve usuarios de su empresa
                var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);
                var users = await _userService.GetAllByCompanyAsync(companyId);
                return Ok(users);
            }

            // SuperAdmin ve todos
            var allUsers = await _userService.GetAllAsync();
            return Ok(allUsers);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetAllByCompany(int companyId)
        {
            var users = await _userService.GetAllByCompanyAsync(companyId);
            return Ok(users);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, UserUpdateRoleRequest request)
        {
            await _userService.UpdateRoleAsync(id, request);
            return NoContent();
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(UserUpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _userService.UpdateProfileAsync(userId, request);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _userService.GetByIdAsync(userId);
            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddBySuperAdmin(UserCreateRequest request)
        {
            try
            {
                var user = await _userService.AddBySuperAdminAsync(request);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (OperationNotAllowedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CrearEmpleado")]
        public async Task<IActionResult> AddByAdmin(UserCreateByAdminRequest request)
        {
            var companyId = int.Parse(User.FindFirst("IdCompany")!.Value);

            try
            {
                var user = await _userService.AddByAdminAsync(request, companyId);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Admin pasa su companyId para validar tenant
                int? companyId = User.IsInRole("Admin")
                    ? int.Parse(User.FindFirst("IdCompany")!.Value)
                    : null;

                await _userService.DeleteAsync(id, companyId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (OperationNotAllowedException)
            {
                return Forbid();
            }
        }
    }
}