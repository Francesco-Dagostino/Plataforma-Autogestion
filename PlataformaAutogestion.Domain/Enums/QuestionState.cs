using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Enums
{
    public class QuestionState
    {
        public enum Roles
        {
            Empleado,
            Admin,
            SuperAdmin
        }

        public enum StatusDay
        {
            Aprobada,
            Desaprobada,
            Pendiente
        }
    }
}
