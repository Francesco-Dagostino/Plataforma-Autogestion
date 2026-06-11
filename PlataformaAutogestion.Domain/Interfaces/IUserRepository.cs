using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllByCompanyAsync(int companyId);
        Task<User?> GetByIdWithDetailsAsync(int id);
    }
}
