using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllAsync();
        Task<List<UserDTO>> GetAllByCompanyAsync(int companyId);
        Task<UserDTO> GetByIdAsync(int id);
        Task<UserDTO> AddAsync(UserCreateRequest request);
        Task UpdateAsync(int id, UserCreateRequest request);
        Task DeleteAsync(int id);
    }
}
