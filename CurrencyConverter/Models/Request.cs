namespace CurrencyConverter.Models
{
    public class Request
    {
        public string? ClientIP { get; set; }
        public string? ClientId { get; set; }
        public string? HTTPMethod { get; set; }
        public string? Endpoint { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseTime { get; set; }
    }
}
