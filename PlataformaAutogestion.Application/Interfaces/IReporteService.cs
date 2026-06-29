using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Application.Interfaces
{
    public interface IReporteService
    {
        Task<byte[]> GenerarPdfRecibosAsync(int liquidationId);
        Task<byte[]> GenerarTxtBancoAsync(int liquidationId);
    }

}