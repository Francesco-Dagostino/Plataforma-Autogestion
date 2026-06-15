using System;
using System.Collections.Generic;
using System.Text;

// UnauthorizedException.cs
namespace PlataformaAutogestion.Domain.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("No autenticado. Debe iniciar sesión.")
        {
        }
    }
}
