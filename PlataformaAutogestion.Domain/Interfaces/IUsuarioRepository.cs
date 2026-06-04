using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<User>
    {
        User? GetByEmailAsync(string email);
        Task<List<User>> GetByEmpresaIdAsync(int empresaId);
        Task<User> GetJornadasAsync(int usuarioId);
    }
}
