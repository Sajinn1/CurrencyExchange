namespace CurrencyExchange.Models
{
    public class AddCurrencyViewModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string? Decimalname { get; set; }

        public string? Nondecimal { get; set; }

        public string Millionlakh { get; set; }

        public char Symbol { get; set; }

    }
}
