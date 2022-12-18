namespace AdminLte.ViewModels
{
    public class FeeLimitForm
    {
        public int CurrencyId { get; set; }
        public int? TransactionTypeId { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal FixedAmount { get; set; }
        public decimal PercentAmount { get; set; }
        public decimal MinLimit { get; set; }
        public decimal? MaxLimit { get; set; }
        public int ProcessingTime { get; set; }
        public bool HasTransaction { get; set; }
    }
}
