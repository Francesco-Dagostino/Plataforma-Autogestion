using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Enums
{
    public class QuestionState
    {
        public enum Roles
        {
            SuperAdmin,
            Admin,
            Empleado
        }

        public enum EstadoJornada
        {
            Aprobada,
            Desaprobada,
            Pendiente
        }
    }
}
