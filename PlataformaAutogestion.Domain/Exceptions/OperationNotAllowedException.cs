using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Exceptions
{
    public class OperationNotAllowedException : Exception
    {
        public OperationNotAllowedException(string message) : base(message) { }
    }
}
