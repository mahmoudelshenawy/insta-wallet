using AdminLte.Data.Enums;

namespace AdminLte.Data.Entities
{
    public class TransactionTypes
    {
        public int Id { get; set; }
        public TransactionTypeEnum Name { get; set; }
        public bool Feeable { get; set; }

        public List<FeeLimit> FeeLimits { get; set; }
    }
}
