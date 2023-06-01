using System.ComponentModel.DataAnnotations;

namespace AdminLte.Areas.User.Models
{
    public class StripeViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public long Month { get; set; }
        [Required]
        public long Year { get; set; }
        [Required]
        public string Cvc { get; set; }
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

        public string PaymentIntentId { get; set; }
        public string RedirectToStripe { get; set; }

    }
}
