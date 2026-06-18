using PlataformaAutogestion.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IDetailLiquidationService
    {
        Task<List<DetailLiquidationDTO>> GetByUserAsync(int userId);
        Task<List<DetailLiquidationDTO>> GetByLiquidationAsync(int idLiquidation, int idCompany);
    }
}
