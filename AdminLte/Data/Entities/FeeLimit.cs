using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class FeeLimit
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public int? TransactionTypeId { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal FixedAmount { get; set; }
        public decimal PercentAmount { get; set; }
        public decimal MinLimit { get; set; }
        public decimal? MaxLimit { get; set; }
        public int ProcessingTime { get; set; }
        public bool HasTransaction { get; set; }
        public Currency Currency { get; set; }

        [ForeignKey("TransactionTypeId")]
        public TransactionTypes TransactionTypes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
