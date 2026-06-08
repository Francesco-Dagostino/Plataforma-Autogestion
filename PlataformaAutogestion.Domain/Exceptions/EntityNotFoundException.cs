using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, object key)
            : base($"No se encontró {entityName} con id {key}.")
        {
        }
    }
}
