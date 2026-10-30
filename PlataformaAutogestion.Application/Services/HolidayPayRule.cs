using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Application.Services
{
    // Regla de pago: las horas trabajadas en fin de semana o feriado usan el valor hora de feriado de la empresa.
    // Si la empresa no definió uno, se paga el doble del valor hora normal.
    // Los "puentes turísticos" son días no laborables optativos, no feriados, así que se pagan normal.
    public static class HolidayPayRule
    {
        public const decimal DefaultMultiplier = 2m;
        private const string BridgeType = "puente";

        // Trae una sola vez los feriados de los años involucrados, en vez de consultar la API por cada jornada.
        public static async Task<HashSet<DateTime>> LoadHolidaysAsync(IHolidayService holidayService, IEnumerable<DateTime> dates)
        {
            var holidays = new HashSet<DateTime>();

            foreach (var year in dates.Select(d => d.Year).Distinct())
            {
                foreach (var holiday in await holidayService.GetHolidaysByYearAsync(year))
                {
                    if (!string.Equals(holiday.Type, BridgeType, StringComparison.OrdinalIgnoreCase))
                        holidays.Add(holiday.Date.Date);
                }
            }

            return holidays;
        }

        public static bool IsHolidayPayDay(DateTime date, HashSet<DateTime> holidays) =>
            date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || holidays.Contains(date.Date);

        // Valor hora que la empresa paga en feriados y fines de semana.
        public static decimal HolidayRate(Company company) =>
            company.HolidayHourlyRate ?? company.ParameterSystem * DefaultMultiplier;

        public static decimal HourlyRateFor(DateTime date, Company company, HashSet<DateTime> holidays) =>
            IsHolidayPayDay(date, holidays) ? HolidayRate(company) : company.ParameterSystem;
    }
}
