namespace AdminLte.Data.Entities
{
    public class CurrencyPaymentMethod
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? ActivatedFor { get; set; }
        public string? MethodData { get; set; }
        public int ProcessingTime { get; set; }

        public Currency Currency { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
