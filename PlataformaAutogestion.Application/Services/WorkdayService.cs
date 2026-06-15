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

        public WorkdayService(IWorkdayRepository workdayRepository, ICompanyRepository companyRepository, IUserRepository userRepository)
        {
            _workdayRepository = workdayRepository;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
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

        public async Task<WorkdayDTO> AddAsync(WorkdayCreateRequest request)
        {
            var company = await _companyRepository.GetByIdAsync(request.IdCompany)
                ?? throw new EntityNotFoundException("Company", request.IdCompany);

            var user = await _userRepository.GetByIdAsync(request.IdUser)
                ?? throw new EntityNotFoundException("User", request.IdUser);

            var workday = new Workday
            {
                Id = Guid.NewGuid().ToString(),
                HoursWorked = request.HoursWorked,
                DateEntry = request.DateEntry,
                DateApproval = null,
                Estado = StatusDay.Pendiente,
                IdUser = request.IdUser,
                IdCompany = request.IdCompany
            };

            await _workdayRepository.AddAsync(workday);
            return WorkdayDTO.FromEntity(workday);
        }

        public async Task ApproveAsync(string id)
        {
            var workday = await _workdayRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workday", id);

            if (workday.Estado != StatusDay.Pendiente)
                throw new InvalidOperationException("Solo se pueden aprobar jornadas en estado Pendiente.");

            workday.Estado = StatusDay.Aprobada;
            workday.DateApproval = DateTime.UtcNow;

            await _workdayRepository.UpdateAsync(workday);
        }

        public async Task RejectAsync(string id)
        {
            var workday = await _workdayRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workday", id);

            if (workday.Estado != StatusDay.Pendiente)
                throw new InvalidOperationException("Solo se pueden rechazar jornadas en estado Pendiente.");

            workday.Estado = StatusDay.Desaprobada;
            workday.DateApproval = DateTime.UtcNow;

            await _workdayRepository.UpdateAsync(workday);
        }
    }
}
