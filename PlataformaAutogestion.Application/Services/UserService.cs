using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
            var company = await _companyRepository.GetByIdAsync(request.IdCompany)
                ?? throw new EntityNotFoundException("Company", request.IdCompany);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                role = Roles.Empleado,
                IdCompany = request.IdCompany,
                CreationDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return UserDTO.FromEntity(user);
        }

        public async Task UpdateAsync(int id, UserCreateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", id);

            user.Name = request.Name;
            user.Email = request.Email;
            user.UserName = request.UserName;
            user.Password = request.Password;
            user.role = request.Role;
            user.IdCompany = request.IdCompany;

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
