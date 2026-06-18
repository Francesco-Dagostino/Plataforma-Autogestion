using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Application.Models.Response;
using PlataformaAutogestion.Domain.Entities;
using System.Threading.Tasks;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface ILiquidationService
    {
        Task<Liquidation> SimularLiquidacionAsync(LiquidationRequest request);
        Task<Liquidation> CerrarMesAsync(LiquidationRequest request);
        Task<SimulacionEmpleadoResponse> SimularSueldoEmpleadoAsync(int userId, int month, int year);
        Task<LiquidacionCierreResponse> GetCierreMesAsync(int companyId, int month, int year);

    }
}