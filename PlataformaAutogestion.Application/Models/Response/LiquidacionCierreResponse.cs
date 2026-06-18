namespace PlataformaAutogestion.Application.Models.Response
{
    public class LiquidacionCierreResponse
    {
        public int LiquidationId { get; set; }
        public DateTime LiquidationDate { get; set; }
        public decimal Total { get; set; }
        public List<DetalleLiquidacionResponse> Detalles { get; set; } = new();
    }
}