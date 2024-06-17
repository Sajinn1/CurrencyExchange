namespace CurrencyExchange.Entities
{
    public class ExchangeRate : Entity
    {
        public Guid Id { get; set; }

        public Guid CurrencyID { get; set; }

        public Currency? Currency { get; set; }

        public decimal BuyRate { get; set; }

        public decimal SalesRate { get; set; }

        public string? User { get; set; }

        public decimal? CommiPer {  get; set; }

        public int? CommiRs { get; set; }
    }
}
