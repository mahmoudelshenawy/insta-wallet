using AdminLte.Data.Entities;

namespace AdminLte.ViewModels
{
    public class FeesLimitFormViewModel
    {
        public string TypeToDisplay { get; set; }

        public string tType { get; set; }
        public List<PaymentMethod>? PaymentMethods { get; set; }

        public FeeLimitForm? SingleForm { get; set; }
        public FeesLimitsForm? FeesLimits { get; set; }
        public int CurrencyId { get; set; }

        public int TransactionTypeId { get; set; }
        public string TransactionType { get; set; }
    }
}
