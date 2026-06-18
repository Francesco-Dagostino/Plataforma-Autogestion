using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Models;
using PlataformaAutogestion.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Services
{
    public class DetailLiquidationService : IDetailLiquidationService
    {
        private readonly IDetailLiquidationRepositoy _detailLiquidationRepository;

        public DetailLiquidationService(IDetailLiquidationRepositoy detailLiquidationRepository)
        {
            _detailLiquidationRepository = detailLiquidationRepository;
        }

        public async Task<List<DetailLiquidationDTO>> GetByUserAsync(int userId)
        {
            var details = await _detailLiquidationRepository.GetByUserAsync(userId);
            return details.Select(DetailLiquidationDTO.FromEntity).ToList();
        }

        public async Task<List<DetailLiquidationDTO>> GetByLiquidationAsync(int idLiquidation, int idCompany)
        {
            var details = await _detailLiquidationRepository.GetByLiquidationAsync(idLiquidation, idCompany);
            return details.Select(DetailLiquidationDTO.FromEntity).ToList();
        }
    }
}
