namespace PlataformaAutogestion.Application.Models.Response
{
    public class SimulacionEmpleadoResponse
    {
        public decimal TotalHoras { get; set; }
        public decimal MontoAcumulado { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }

        public decimal ValorHoraActual { get; set; }
    }
}