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
            return users.Select(UserDTO.FromEntity).ToList();
        }

        public async Task<List<UserDTO>> GetAllByCompanyAsync(int companyId)
        {
            var users = await _userRepository.GetAllByCompanyAsync(companyId);
            return users.Select(UserDTO.FromEntity).ToList();
        }

        public async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            return UserDTO.FromEntity(user);
        }

        public async Task<UserDTO> AddAsync(UserCreateRequest request)
        {
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

        public async Task UpdateRoleAsync(int id, UserUpdateRoleRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            if (request.Role == Roles.SuperAdmin)
                throw new OperationNotAllowedException("No se puede asignar el rol SuperAdmin desde esta operación.");

            user.role = request.Role;

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateProfileAsync(int id, UserUpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            user.Name = request.Name;
            user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = PasswordHasher.Hash(request.Password);

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            await _userRepository.DeleteAsync(user);
        }
    }
}