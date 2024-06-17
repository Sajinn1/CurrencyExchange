using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Entities
{
    public class Entity
    {
        public string? Tag { get; set; } = string.Empty;
        public bool RecordStatus { get; set; } = true;
        public string? Remarks { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
