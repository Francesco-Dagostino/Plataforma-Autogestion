using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Application.Services
{
    public class WorkdayService : IWorkdayService
    {
        private readonly IWorkdayRepository _workdayRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHolidayService _holidayService;

        public WorkdayService(IWorkdayRepository workdayRepository, ICompanyRepository companyRepository, IUserRepository userRepository, IHolidayService holidayService)
        {
            _workdayRepository = workdayRepository;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _holidayService = holidayService;
        }

        public async Task<List<WorkdayDTO>> GetByUserAsync(int userId)
        {
            var workdays = await _workdayRepository.GetByUserAsync(userId);
            return workdays.Select(WorkdayDTO.FromEntity).ToList();
        }

        public async Task<List<WorkdayDTO>> GetPendingByCompanyAsync(int companyId)
        {
            var workdays = await _workdayRepository.GetPendingByCompanyAsync(companyId);
            return workdays.Select(WorkdayDTO.FromEntity).ToList();
        }

        public async Task<WorkdayDTO> AddAsync(WorkdayCreateRequest request, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new EntityNotFoundException("User", userId);

            var idCompany = user.IdCompany
                ?? throw new OperationNotAllowedException("El usuario no tiene una empresa asociada, no puede registrar jornadas.");

            var company = await _companyRepository.GetByIdAsync(idCompany)
                ?? throw new EntityNotFoundException("Company", idCompany);

            if (await _holidayService.IsHolidayAsync(request.DateEntry))
                throw new OperationNotAllowedException("Ese día es o fue feriado no laboral.");

            var workday = new Workday
            {
                Id = Guid.NewGuid().ToString(),
                HoursWorked = request.HoursWorked,
                DateEntry = request.DateEntry,
                DateApproval = null,
                Estado = StatusDay.Pendiente,
                IdUser = userId,
                IdCompany = idCompany
            };

            await _workdayRepository.AddAsync(workday);
            return WorkdayDTO.FromEntity(workday);
        }

        public async Task ApproveAsync(string id, int companyId)
        {
            var workday = await _workdayRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workday", id);

            // Verifica que la jornada pertenezca a la empresa del admin
            if (workday.IdCompany != companyId)
                throw new UnauthorizedAccessException("La jornada no pertenece a tu empresa.");

            if (workday.Estado != StatusDay.Pendiente)
                throw new InvalidOperationException("Solo se pueden aprobar jornadas en estado Pendiente.");

            workday.Estado = StatusDay.Aprobada;
            workday.DateApproval = DateTime.UtcNow;

            await _workdayRepository.UpdateAsync(workday);
        }

        public async Task RejectAsync(string id, int companyId)
        {
            var workday = await _workdayRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workday", id);

            // Verifica que la jornada pertenezca a la empresa del admin
            if (workday.IdCompany != companyId)
                throw new UnauthorizedAccessException("La jornada no pertenece a tu empresa.");

            if (workday.Estado != StatusDay.Pendiente)
                throw new InvalidOperationException("Solo se pueden rechazar jornadas en estado Pendiente.");

            workday.Estado = StatusDay.Desaprobada;
            workday.DateApproval = DateTime.UtcNow;

            await _workdayRepository.UpdateAsync(workday);
        }
    }
}
