using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Application.Models.Response;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Services
{
    public class LiquidationService : ILiquidationService
    {
        private readonly ILiquidationRepository _liquidationRepository;
        private readonly IWorkdayRepository _workdayRepository;
        private readonly IHolidayService _holidayService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;

        public LiquidationService(
            ILiquidationRepository liquidationRepository,
            IWorkdayRepository workdayRepository,
            IHolidayService holidayService,
            ICompanyRepository companyRepository,
            IUserRepository userRepository)
        {
            _liquidationRepository = liquidationRepository;
            _workdayRepository = workdayRepository;
            _holidayService = holidayService;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
        }

        public async Task<SimulacionEmpleadoResponse> SimularSueldoEmpleadoAsync(int userId, int month, int year)
        {
            if (month < 1 || month > 12)
                throw new InvalidOperationException("Mes inválido.");

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new EntityNotFoundException("User", userId);

            if (user.IdCompany == null)
                throw new InvalidOperationException("El usuario no tiene empresa asignada.");

            var company = await _companyRepository.GetByIdAsync(user.IdCompany.Value)
                ?? throw new EntityNotFoundException("Company", user.IdCompany.Value);

            decimal valorHoraEmpresa = company.ParameterSystem;
            var allWorkdays = await _workdayRepository.GetByUserAsync(userId);

            var jornadasDelMes = allWorkdays
                .Where(w =>
                    w.DateEntry.Month == month &&
                    w.DateEntry.Year == year &&
                    w.Estado != StatusDay.Desaprobada)
                .ToList();

            decimal totalHoras = 0;
            decimal montoAcumulado = 0;

            foreach (var jornada in jornadasDelMes)
            {
                totalHoras += jornada.HoursWorked;
                decimal valorHoraAplicado = valorHoraEmpresa;

                if (await _holidayService.IsHolidayAsync(jornada.DateEntry))
                {
                    valorHoraAplicado *= 2;
                }

                montoAcumulado += jornada.HoursWorked * valorHoraAplicado;
            }

            return new SimulacionEmpleadoResponse
            {
                TotalHoras = totalHoras,
                MontoAcumulado = montoAcumulado,
                Mes = month,
                Anio = year,
                ValorHoraActual = valorHoraEmpresa
            };
        }

        public async Task<Liquidation> SimularLiquidacionAsync(int companyId, LiquidationRequest request)
        {
            return await GenerarLiquidacionCoreAsync(companyId, request, false);
        }

        public async Task<Liquidation> CerrarMesAsync(int companyId, LiquidationRequest request)
        {
            if (await _liquidationRepository.ExistsLiquidationForPeriodAsync(companyId, request.Month, request.Year))
                throw new InvalidOperationException("Ya existe una liquidación cerrada para este período. Si deseas generarla de nuevo, primero debes anular la actual.");

            return await GenerarLiquidacionCoreAsync(companyId, request, true);
        }

        private async Task<Liquidation> GenerarLiquidacionCoreAsync(int companyId, LiquidationRequest request, bool guardarEnBd)
        {
            if (request.Month < 1 || request.Month > 12)
                throw new InvalidOperationException("Mes inválido.");

            var company = await _companyRepository.GetByIdAsync(companyId)
                ?? throw new EntityNotFoundException("Company", companyId);

            decimal valorHoraEmpresa = company.ParameterSystem;

            if (valorHoraEmpresa <= 0)
                throw new InvalidOperationException("La empresa no tiene configurado un valor hora válido.");

            var allWorkdays = await _workdayRepository.GetAllByCompanyAsync(companyId);

            var jornadasDelMes = allWorkdays
                .Where(w => w.Estado == StatusDay.Aprobada
                         && w.DateEntry.Month == request.Month
                         && w.DateEntry.Year == request.Year)
                .ToList();

            if (!jornadasDelMes.Any())
                throw new InvalidOperationException("No hay jornadas aprobadas para liquidar.");

            var liquidation = new Liquidation
            {
                LiquidationDate = DateTime.SpecifyKind(
                    new DateTime(request.Year, request.Month, DateTime.DaysInMonth(request.Year, request.Month)),
                    DateTimeKind.Utc),
                ExecutionDate = DateTime.UtcNow, // <-- SOLUCIÓN A FRAN: Fecha exacta del clic
                IdCompany = companyId,
                IsClosed = guardarEnBd,
                detailLiquidations = new()
            };

            decimal totalLiquidacionEmpresa = 0;
            var jornadasPorUsuario = jornadasDelMes.GroupBy(w => w.IdUser);

            foreach (var group in jornadasPorUsuario)
            {
                decimal totalHorasUsuario = 0;
                decimal montoTotalUsuario = 0;

                foreach (var jornada in group)
                {
                    totalHorasUsuario += jornada.HoursWorked;
                    decimal valorHoraAplicado = valorHoraEmpresa;

                    if (await _holidayService.IsHolidayAsync(jornada.DateEntry))
                        valorHoraAplicado *= 2;

                    montoTotalUsuario += jornada.HoursWorked * valorHoraAplicado;
                }

                liquidation.detailLiquidations.Add(new DetailLiquidation
                {
                    IdUser = group.Key,
                    IdCompany = companyId,
                    TotalHours = totalHorasUsuario,
                    Amount = montoTotalUsuario
                });

                totalLiquidacionEmpresa += montoTotalUsuario;
            }

            liquidation.Total = totalLiquidacionEmpresa;

            if (guardarEnBd)
                await _liquidationRepository.AddAsync(liquidation);

            return liquidation;
        }

        public async Task<LiquidacionCierreResponse> GetCierreMesAsync(int companyId, int month, int year)
        {
            var liquidacion = await _liquidationRepository.GetByPeriodAsync(companyId, month, year)
                ?? throw new EntityNotFoundException("Liquidation", 0);

            return new LiquidacionCierreResponse
            {
                LiquidationId = liquidacion.Id,
                LiquidationDate = liquidacion.LiquidationDate,
                Total = liquidacion.Total,
                Detalles = liquidacion.detailLiquidations.Select(d => new DetalleLiquidacionResponse
                {
                    IdUser = d.IdUser,
                    // SOLUCIÓN BUG: Trae el nombre del usuario o avisa si está nulo
                    NombreEmpleado = d.User != null ? d.User.Name : "Desconocido",
                    TotalHoras = d.TotalHours,
                    Monto = d.Amount
                }).ToList()
            };
        }

        // rehacer mes
        public async Task DeleteCierreMesAsync(int companyId, int liquidationId)
        {
            var liquidation = await _liquidationRepository.GetByIdAsync(liquidationId)
                ?? throw new EntityNotFoundException("Liquidation", liquidationId);

            if (liquidation.IdCompany != companyId)
                throw new UnauthorizedAccessException("La liquidación no pertenece a tu empresa.");

            await _liquidationRepository.DeleteAsync(liquidation);
        }
    }
}