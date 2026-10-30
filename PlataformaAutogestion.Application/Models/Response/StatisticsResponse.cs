namespace PlataformaAutogestion.Application.Models.Response
{
    public class EmployeeStatisticsResponse
    {
        public int IdUser { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal ApprovedHours { get; set; }
        public decimal PendingHours { get; set; }
        public decimal RejectedHours { get; set; }
        public int ApprovedDays { get; set; }
        // Promedio de horas aprobadas por día trabajado.
        public decimal AverageHoursPerDay { get; set; }
        // Aprobadas sobre las resueltas (aprobadas + rechazadas), en porcentaje; null si no hay resueltas.
        public decimal? ApprovalRate { get; set; }
        // Horas aprobadas por el valor hora actual de la empresa.
        public decimal Cost { get; set; }
        // Porcentaje del costo total del período.
        public decimal CostShare { get; set; }
    }

    public class DailyStatisticsResponse
    {
        public string Date { get; set; } = string.Empty;
        public decimal ApprovedHours { get; set; }
        public decimal PendingHours { get; set; }
    }

    public class StatisticsResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal HourlyRate { get; set; }
        // Valor hora que se paga en feriados y fines de semana.
        public decimal HolidayHourlyRate { get; set; }
        public decimal TotalApprovedHours { get; set; }
        public decimal TotalPendingHours { get; set; }
        public decimal TotalRejectedHours { get; set; }
        public decimal TotalCost { get; set; }
        public decimal? ApprovalRate { get; set; }
        public List<EmployeeStatisticsResponse> Employees { get; set; } = new();
        public List<DailyStatisticsResponse> Daily { get; set; } = new();
    }
}
