using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Response;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using System.Globalization;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IWorkdayRepository _workdayRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IHolidayService _holidayService;

        public StatisticsService(IWorkdayRepository workdayRepository, ICompanyRepository companyRepository, IHolidayService holidayService)
        {
            _workdayRepository = workdayRepository;
            _companyRepository = companyRepository;
            _holidayService = holidayService;
        }

        public async Task<StatisticsResponse> GetMonthlyAsync(int companyId, int month, int year)
        {
            if (month < 1 || month > 12)
                throw new OperationNotAllowedException("El mes debe estar entre 1 y 12.");
            if (year < 2000 || year > 2100)
                throw new OperationNotAllowedException("El año no es válido.");

            var company = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new EntityNotFoundException("Company", companyId);

            // Mismo criterio de mes que la liquidación: por la fecha (UTC) de la jornada.
            var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var workdays = await _workdayRepository.GetByCompanyAndPeriodAsync(companyId, from, from.AddMonths(1));

            var rate = company.ParameterSystem;
            var feriados = await HolidayPayRule.LoadHolidaysAsync(_holidayService, workdays.Select(w => w.DateEntry));

            var employees = workdays
                .GroupBy(w => new { w.IdUser, Name = w.Usuario?.Name ?? string.Empty })
                .Select(g =>
                {
                    var approved = g.Where(w => w.Estado == StatusDay.Aprobada).ToList();
                    var approvedHours = approved.Sum(w => w.HoursWorked);
                    var rejectedHours = g.Where(w => w.Estado == StatusDay.Desaprobada).Sum(w => w.HoursWorked);
                    var resolved = approvedHours + rejectedHours;

                    return new EmployeeStatisticsResponse
                    {
                        IdUser = g.Key.IdUser,
                        Name = g.Key.Name,
                        ApprovedHours = approvedHours,
                        PendingHours = g.Where(w => w.Estado == StatusDay.Pendiente).Sum(w => w.HoursWorked),
                        RejectedHours = rejectedHours,
                        ApprovedDays = approved.Count,
                        AverageHoursPerDay = approved.Count == 0 ? 0 : Math.Round(approvedHours / approved.Count, 2),
                        ApprovalRate = resolved == 0 ? null : Math.Round(approvedHours / resolved * 100, 1),
                        // Mismo criterio que la liquidación: en fin de semana o feriado rige el valor hora de feriado.
                        Cost = Math.Round(approved.Sum(w => w.HoursWorked * HolidayPayRule.HourlyRateFor(w.DateEntry, company, feriados)), 2)
                    };
                })
                .OrderByDescending(e => e.ApprovedHours)
                .ThenBy(e => e.Name)
                .ToList();

            var totalCost = employees.Sum(e => e.Cost);
            foreach (var e in employees)
                e.CostShare = totalCost == 0 ? 0 : Math.Round(e.Cost / totalCost * 100, 1);

            var totalApproved = employees.Sum(e => e.ApprovedHours);
            var totalRejected = employees.Sum(e => e.RejectedHours);
            var totalResolved = totalApproved + totalRejected;

            var daily = workdays
                .Where(w => w.Estado != StatusDay.Desaprobada)
                .GroupBy(w => w.DateEntry.ToUniversalTime().Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyStatisticsResponse
                {
                    Date = g.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ApprovedHours = g.Where(w => w.Estado == StatusDay.Aprobada).Sum(w => w.HoursWorked),
                    PendingHours = g.Where(w => w.Estado == StatusDay.Pendiente).Sum(w => w.HoursWorked)
                })
                .ToList();

            return new StatisticsResponse
            {
                Month = month,
                Year = year,
                HourlyRate = rate,
                HolidayHourlyRate = HolidayPayRule.HolidayRate(company),
                TotalApprovedHours = totalApproved,
                TotalPendingHours = employees.Sum(e => e.PendingHours),
                TotalRejectedHours = totalRejected,
                TotalCost = totalCost,
                ApprovalRate = totalResolved == 0 ? null : Math.Round(totalApproved / totalResolved * 100, 1),
                Employees = employees,
                Daily = daily
            };
        }
    }
}
