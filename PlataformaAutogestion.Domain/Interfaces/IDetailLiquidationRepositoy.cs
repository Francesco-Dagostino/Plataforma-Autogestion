using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Domain.Interfaces
{
    public interface IDetailLiquidationRepositoy : IBaseRepository<DetailLiquidation>
    {
        Task<List<DetailLiquidation>> GetByUserAsync(int UserId);
        Task<List<DetailLiquidation>> GetByLiquidationAsync(int idLiquidation, int idCompany);
    }
}
