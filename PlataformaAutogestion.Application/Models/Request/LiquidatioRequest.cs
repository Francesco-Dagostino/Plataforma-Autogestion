namespace PlataformaAutogestion.Application.Models.Request
{
    public class LiquidationRequest
    {
        public int CompanyId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}