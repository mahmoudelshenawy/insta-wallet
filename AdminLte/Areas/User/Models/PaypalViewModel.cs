namespace AdminLte.Areas.User.Models
{
    public class PaypalViewModel
    {
        public string ClientId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }

        public decimal? FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal? TotalFees { get; set; }

        public bool EarlyPay { get; set; } = true;

        public string Status { get; set; }

        //Response action payment
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
