using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException()
            : base("No tiene permisos para realizar esta acción.")
        {
        }

        public ForbiddenException(string role)
            : base($"Esta acción requiere el rol '{role}'.")
        {
        }
    }
}
