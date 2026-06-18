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

        public async Task<SimulacionEmpleadoResponse> SimularSueldoEmpleadoAsync(
            int userId,
            int month,
            int year)
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
                Anio = year
            };
        }

        public async Task<Liquidation> SimularLiquidacionAsync(LiquidationRequest request)
        {
            return await GenerarLiquidacionCoreAsync(request, false);
        }

        public async Task<Liquidation> CerrarMesAsync(LiquidationRequest request)
        {
            if (await _liquidationRepository.ExistsLiquidationForPeriodAsync(
                request.CompanyId,
                request.Month,
                request.Year))
            {
                throw new InvalidOperationException(
                    "Ya existe una liquidación cerrada para este período.");
            }

            return await GenerarLiquidacionCoreAsync(request, true);
        }

        private async Task<Liquidation> GenerarLiquidacionCoreAsync(
            LiquidationRequest request,
            bool guardarEnBd)
        {
            if (request.Month < 1 || request.Month > 12)
                throw new InvalidOperationException("Mes inválido.");

            var company = await _companyRepository.GetByIdAsync(request.CompanyId)
                ?? throw new EntityNotFoundException("Company", request.CompanyId);

            decimal valorHoraEmpresa = company.ParameterSystem;

            if (valorHoraEmpresa <= 0)
                throw new InvalidOperationException(
                    "La empresa no tiene configurado un valor hora válido.");

            var allWorkdays = await _workdayRepository
                .GetAllByCompanyAsync(request.CompanyId);

            var jornadasDelMes = allWorkdays
                .Where(w =>
                    w.Estado == StatusDay.Aprobada &&
                    w.DateEntry.Month == request.Month &&
                    w.DateEntry.Year == request.Year)
                .ToList();

            if (!jornadasDelMes.Any())
                throw new InvalidOperationException(
                    "No hay jornadas aprobadas para liquidar.");

            var liquidation = new Liquidation
            {
                LiquidationDate = new DateTime(
                    request.Year,
                    request.Month,
                    DateTime.DaysInMonth(request.Year, request.Month)),
                IdCompany = request.CompanyId,
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
                    {
                        valorHoraAplicado *= 2;
                    }

                    montoTotalUsuario += jornada.HoursWorked * valorHoraAplicado;
                }

                liquidation.detailLiquidations.Add(new DetailLiquidation
                {
                    IdUser = group.Key,
                    IdCompany = request.CompanyId,
                    TotalHours = totalHorasUsuario,
                    Amount = montoTotalUsuario
                });

                totalLiquidacionEmpresa += montoTotalUsuario;
            }

            liquidation.Total = totalLiquidacionEmpresa;

            if (guardarEnBd)
            {
                await _liquidationRepository.AddAsync(liquidation);
            }

            return liquidation;
        }
    }
}