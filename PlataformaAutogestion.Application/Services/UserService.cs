using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using PlataformaAutogestion.Domain.Security;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => u.IsActive).Select(UserDTO.FromEntity).ToList();
        }

        public async Task<List<UserDTO>> GetAllByCompanyAsync(int companyId)
        {
            var users = await _userRepository.GetAllByCompanyAsync(companyId);
            return users.Where(u => u.IsActive).Select(UserDTO.FromEntity).ToList();
        }

        public async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            return UserDTO.FromEntity(user);
        }

        private async Task EnsureUniqueAsync(string userName, string email)
        {
            if (await _userRepository.GetByUserNameAsync(userName) != null)
                throw new OperationNotAllowedException("Ya existe un usuario con ese nombre de usuario.");

            if (await _userRepository.GetByEmailAsync(email) != null)
                throw new OperationNotAllowedException("Ya existe un usuario con ese correo.");
        }

        public async Task<UserDTO> AddBySuperAdminAsync(UserCreateRequest request)
        {
            await EnsureUniqueAsync(request.UserName, request.Email);

            if (request.Role == Roles.SuperAdmin)
            {
                if (request.IdCompany != null)
                    throw new OperationNotAllowedException("SuperAdmin no debe tener empresa asociada.");
            }
            else
            {
                if (request.IdCompany == null)
                    throw new OperationNotAllowedException("IdCompany es requerido para este rol.");

                _ = await _companyRepository.GetByIdAsync(request.IdCompany.Value)
                    ?? throw new EntityNotFoundException("Company", request.IdCompany.Value);
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.UserName,
                Password = PasswordHasher.Hash(request.Password),
                role = request.Role,
                IdCompany = request.IdCompany,
                CreationDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return UserDTO.FromEntity(user);
        }

        public async Task<UserDTO> AddByAdminAsync(UserCreateByAdminRequest request, int companyId)
        {
            _ = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new EntityNotFoundException("Company", companyId);

            await EnsureUniqueAsync(request.UserName, request.Email);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.UserName,
                Password = PasswordHasher.Hash(request.Password),
                role = Roles.Empleado,
                IdCompany = companyId,
                CreationDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return UserDTO.FromEntity(user);
        }

        public async Task UpdateRoleAsync(int id, UserUpdateRoleRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            if (request.Role == Roles.SuperAdmin)
                throw new OperationNotAllowedException("No se puede asignar el rol SuperAdmin desde esta operación.");

            user.role = request.Role;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateEmployeeAsync(int id, UserUpdateByAdminRequest request, int companyId)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            if (user.IdCompany != companyId || user.role != Roles.Empleado)
                throw new ForbiddenException();

            var userNameOwner = await _userRepository.GetByUserNameAsync(request.UserName);
            if (userNameOwner != null && userNameOwner.Id != id)
                throw new OperationNotAllowedException("Ya existe un usuario con ese nombre de usuario.");

            var emailOwner = await _userRepository.GetByEmailAsync(request.Email);
            if (emailOwner != null && emailOwner.Id != id)
                throw new OperationNotAllowedException("Ya existe un usuario con ese correo.");

            user.Name = request.Name;
            user.Email = request.Email;
            user.UserName = request.UserName;

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = PasswordHasher.Hash(request.Password);

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateProfileAsync(int id, UserUpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            var emailOwner = await _userRepository.GetByEmailAsync(request.Email);
            if (emailOwner != null && emailOwner.Id != id)
                throw new OperationNotAllowedException("Ya existe un usuario con ese correo.");

            user.Name = request.Name;
            user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = PasswordHasher.Hash(request.Password);

            await _userRepository.UpdateAsync(user);
        }

        // Devuelve true si se dio de baja (baja lógica) y false si se eliminó definitivamente
        public async Task<bool> DeleteAsync(int id, int? companyId = null)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            // Verifica aislamiento multitenant cuando lo llama un Admin
            if (companyId.HasValue && user.IdCompany != companyId.Value)
                throw new OperationNotAllowedException("No puedes eliminar usuarios de otra empresa.");

            if (companyId.HasValue && user.role != Roles.Empleado)
                throw new ForbiddenException();

            // Con historial no se borra: se da de baja para no romper liquidaciones ya cerradas
            if (await _userRepository.HasRelatedDataAsync(id))
            {
                user.IsActive = false;
                await _userRepository.UpdateAsync(user);
                return true;
            }

            await _userRepository.DeleteAsync(user);
            return false;
        }
    }
}