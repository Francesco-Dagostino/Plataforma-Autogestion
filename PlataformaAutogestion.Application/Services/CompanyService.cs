using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Application.Models.Requests;
using PlataformaAutogestion.Domain.Entities;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private const int MaxHistoryItems = 50;
        public const string RateTypeNormal = "Normal";
        public const string RateTypeHoliday = "Feriado";

        private readonly ICompanyRepository _repository;
        private readonly IRateHistoryRepository _rateHistoryRepository;

        public CompanyService(ICompanyRepository repository, IRateHistoryRepository rateHistoryRepository)
        {
            _repository = repository;
            _rateHistoryRepository = rateHistoryRepository;
        }

        public async Task<CompanyDTO> CreateAsync(CompanyCreateRequest dto)
        {
            var company = new Company
            {
                Name = dto.Name,
                Cuit = dto.Cuit,
                DateHigh = dto.DateHigh,
                ParameterSystem = 0,
            };

            await _repository.AddAsync(company);
            return CompanyDTO.FromEntity(company);
        }

        public async Task<List<CompanyDTO>> GetAllAsync()
        {
            var companies = await _repository.GetAllAsync();
            return companies.Select(c => CompanyDTO.FromEntity(c)).ToList();
        }

        public async Task<CompanyDTO> GetByIdAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);
            if (company == null)
                throw new EntityNotFoundException("Company", id);
            return CompanyDTO.FromEntity(company);
        }
        public async Task UpdateAsync(int id, CompanyCreateRequest dto)
        {
            var company = await _repository.GetByIdAsync(id);
            if (company == null)
                throw new EntityNotFoundException("Company", id);

            company.Name = dto.Name;
            company.Cuit = dto.Cuit;
            company.DateHigh = dto.DateHigh;

            await _repository.UpdateAsync(company);
        }

        public async Task UpdateProfileAsync(int id, CompanyProfileRequest dto)
        {
            var company = await _repository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Company", id);

            company.Name = dto.Name;
            company.Cuit = dto.Cuit;

            await _repository.UpdateAsync(company);
        }

        public async Task UpdateHourlyRateAsync(int id, CompanyHourlyRateRequest dto, int? userId, string? userName)
        {
            var company = await _repository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Company", id);

            var previous = company.ParameterSystem;
            var previousHoliday = HolidayPayRule.HolidayRate(company);
            company.ParameterSystem = dto.ParameterSystem;

            await _repository.UpdateAsync(company);

            if (previous != dto.ParameterSystem)
                await RecordRateChangeAsync(id, RateTypeNormal, dto.ParameterSystem, previous, userId, userName);

            // Si los feriados se pagan el doble, su monto real también cambió aunque nadie lo haya tocado.
            var newHoliday = HolidayPayRule.HolidayRate(company);
            if (previousHoliday != newHoliday)
                await RecordRateChangeAsync(id, RateTypeHoliday, newHoliday, previousHoliday, userId, userName);
        }

        public async Task UpdateHolidayRateAsync(int id, CompanyHolidayRateRequest dto, int? userId, string? userName)
        {
            var company = await _repository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Company", id);

            var previous = HolidayPayRule.HolidayRate(company);

            // null vuelve al criterio por defecto: el doble del valor hora.
            company.HolidayHourlyRate = dto.HolidayHourlyRate;

            await _repository.UpdateAsync(company);

            // Se registra el monto que realmente se paga, no la forma de calcularlo.
            var current = HolidayPayRule.HolidayRate(company);
            if (previous != current)
                await RecordRateChangeAsync(id, RateTypeHoliday, current, previous, userId, userName);
        }

        public async Task<List<RateHistoryDTO>> GetRateHistoryAsync(int companyId)
        {
            var items = await _rateHistoryRepository.GetLatestByCompanyAsync(companyId, MaxHistoryItems);
            return items.Select(RateHistoryDTO.FromEntity).ToList();
        }

        // Solo se registra cuando el valor realmente cambia.
        private Task RecordRateChangeAsync(int companyId, string rateType, decimal? value, decimal? previous, int? userId, string? userName)
        {
            return _rateHistoryRepository.AddAsync(new RateHistory
            {
                IdCompany = companyId,
                RateType = rateType,
                Value = value,
                PreviousValue = previous,
                ChangedAt = DateTime.UtcNow,
                IdUser = userId,
                UserName = userName
            });
        }

        public async Task UpdateDailyLimitAsync(int id, CompanyDailyLimitRequest dto)
        {
            var company = await _repository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Company", id);

            company.DailyHoursLimit = dto.DailyHoursLimit;

            await _repository.UpdateAsync(company);
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);
            if (company == null)
                throw new EntityNotFoundException("Company", id);

            if (await _repository.HasRelatedDataAsync(id))
                throw new OperationNotAllowedException("No se puede eliminar una empresa que tiene usuarios o datos asociados.");

            await _repository.DeleteAsync(company);
        }
    }
}
