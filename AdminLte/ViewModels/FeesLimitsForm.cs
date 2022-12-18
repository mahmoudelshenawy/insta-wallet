namespace AdminLte.ViewModels
{
    public class FeesLimitsForm
    {
        public int CurrencyId { get; set; }
        public int? TransactionTypeId { get; set; }
        public List<int>? PaymentMethodId { get; set; }
        public List<decimal> FixedAmount { get; set; }
        public List<decimal> PercentAmount { get; set; }
        public List<decimal> MinLimit { get; set; }
        public List<decimal?> MaxLimit { get; set; }
        public List<int> ProcessingTime { get; set; }
        public List<bool> HasTransaction { get; set; }
    }
}
