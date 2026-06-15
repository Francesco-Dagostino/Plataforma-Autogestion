using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string userName, string password);
    }
}