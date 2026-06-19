using PlataformaAutogestion.Application.Models.Request;
using PlataformaAutogestion.Application.Models.Response;
using PlataformaAutogestion.Domain.Entities;
using System.Threading.Tasks;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface ILiquidationService
    {
        Task<Liquidation> SimularLiquidacionAsync(int companyId, LiquidationRequest request);
        Task<Liquidation> CerrarMesAsync(int companyId, LiquidationRequest request);
        Task<SimulacionEmpleadoResponse> SimularSueldoEmpleadoAsync(int userId, int month, int year);
        Task<LiquidacionCierreResponse> GetCierreMesAsync(int companyId, int month, int year);

        Task DeleteCierreMesAsync(int companyId, int liquidationId);
    }
}