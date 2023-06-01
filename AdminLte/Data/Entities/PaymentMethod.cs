using AdminLte.Data.Enums;
using AdminLte.Rules;

namespace AdminLte.Data.Entities
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public PaymentMethodStatusEnum Status { get; set; }

        public List<FeeLimit> FeeLimits { get; set; }
        public List<Deposit> Deposits { get; set; }
        public List<Transaction> Transactions { get; set; }

    }
}
