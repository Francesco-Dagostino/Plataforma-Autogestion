using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Domain.Exceptions;
using PlataformaAutogestion.Domain.Interfaces;
using PlataformaAutogestion.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text;

namespace PlataformaAutogestion.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ILiquidationRepository _liquidationRepository;
        private readonly IDetailLiquidationRepositoy _detailLiquidationRepository;

        public ReporteService(ILiquidationRepository liquidationRepository, IDetailLiquidationRepositoy detailLiquidationRepository)
        {
            _liquidationRepository = liquidationRepository;
            _detailLiquidationRepository = detailLiquidationRepository;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerarPdfRecibosAsync(int liquidationId)
        {
            var liquidation = await _liquidationRepository.GetByIdWithDetailsAsync(liquidationId)
                ?? throw new EntityNotFoundException("Liquidation", liquidationId);

            if (liquidation.detailLiquidations == null || !liquidation.detailLiquidations.Any())
                throw new InvalidOperationException("La liquidación no tiene detalles para generar recibos.");

            return GeneratePdf(liquidation.LiquidationDate, liquidation.detailLiquidations.ToList());
        }

        public async Task<byte[]> GenerarPdfReciboEmpleadoAsync(int detailId, int userId)
        {
            var detail = await _detailLiquidationRepository.GetByIdAsync(detailId);
            if (detail == null || detail.IdUser != userId)
                throw new EntityNotFoundException("Recibo", detailId);

            var liquidation = await _liquidationRepository.GetByIdWithDetailsAsync(detail.IdLiquidation)
                ?? throw new EntityNotFoundException("Liquidation", detail.IdLiquidation);

            if (!liquidation.IsClosed)
                throw new InvalidOperationException("El recibo solo está disponible para liquidaciones cerradas.");

            var employeeDetail = liquidation.detailLiquidations
                .SingleOrDefault(item => item.Id == detailId && item.IdUser == userId)
                ?? throw new EntityNotFoundException("Recibo", detailId);

            return GeneratePdf(liquidation.LiquidationDate, new List<DetailLiquidation> { employeeDetail });
        }

        private static byte[] GeneratePdf(DateTime liquidationDate, List<DetailLiquidation> detalles)
        {
            var culture = CultureInfo.GetCultureInfo("es-AR");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Recibo de Sueldo")
                        .FontSize(20).Bold().AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(20);

                        for (int i = 0; i < detalles.Count; i++)
                        {
                            var detalle = detalles[i];

                            col.Item().Column(recibo =>
                            {
                                recibo.Spacing(10);

                                recibo.Item().Text($"Período: {liquidationDate:MM/yyyy}");
                                recibo.Item().Text($"Empleado: {detalle.User?.Name ?? "N/A"}");
                                recibo.Item().Text($"Legajo (Id Usuario): {detalle.IdUser}");
                                recibo.Item().LineHorizontal(1);

                                recibo.Item().Text($"Total horas trabajadas: {detalle.TotalHours.ToString("0.##", culture)}");
                                recibo.Item().Text($"Monto a percibir: {detalle.Amount.ToString("C", culture)}");

                                recibo.Item().LineHorizontal(1);
                                recibo.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy}");
                            });

                            if (i < detalles.Count - 1)
                            {
                                col.Item().PageBreak();
                            }
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Generado automáticamente por PlataformaAutogestion");
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerarTxtBancoAsync(int liquidationId)
        {
            var liquidation = await _liquidationRepository.GetByIdWithDetailsAsync(liquidationId)
                ?? throw new EntityNotFoundException("Liquidation", liquidationId);

            if (liquidation.detailLiquidations == null || !liquidation.detailLiquidations.Any())
                throw new InvalidOperationException("La liquidación no tiene detalles para generar el lote bancario.");

            var sb = new StringBuilder();
            sb.AppendLine("CBU;Nombre;Monto");

            foreach (var detalle in liquidation.detailLiquidations)
            {
                var cbuFicticio = $"000{detalle.IdUser:D17}";
                var nombre = detalle.User?.Name ?? "N/A";
                var monto = detalle.Amount.ToString("0.00", CultureInfo.InvariantCulture);

                sb.AppendLine($"{cbuFicticio};{nombre};{monto}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
