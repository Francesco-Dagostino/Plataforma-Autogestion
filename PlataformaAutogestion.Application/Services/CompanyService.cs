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
        private readonly ICompanyRepository _repository;

        public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
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

        public async Task UpdateHourlyRateAsync(int id, CompanyHourlyRateRequest dto)
        {
            var company = await _repository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Company", id);

            company.ParameterSystem = dto.ParameterSystem;

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
