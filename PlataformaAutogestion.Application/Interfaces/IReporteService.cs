using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IReporteService
    {
        Task<byte[]> GenerarPdfRecibosAsync(int liquidationId);
        Task<byte[]> GenerarPdfRecibosEmpresaAsync(int liquidationId, int companyId);
        Task<byte[]> GenerarPdfReciboEmpleadoAsync(int detailId, int userId);
        Task<byte[]> GenerarTxtBancoAsync(int liquidationId);
        Task<byte[]> GenerarTxtBancoEmpresaAsync(int liquidationId, int companyId);
    }

}
