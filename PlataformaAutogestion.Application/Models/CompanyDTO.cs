using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Models
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public long Cuit { get; set; }
        public DateTime DateHigh { get; set; }
        public int ParameterSystem { get; set; }

        public static CompanyDTO FromEntity(Company company)
        {
            return new CompanyDTO
            {
                Id = company.Id,
                Name = company.Name,
                Cuit = company.Cuit,
                DateHigh = company.DateHigh,
                ParameterSystem = company.ParameterSystem
            };
        }
    }
}
