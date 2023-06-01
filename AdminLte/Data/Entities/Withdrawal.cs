using AdminLte.Data.Enums;
using AdminLte.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class Withdrawal
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public int? PayoutSettingId { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? Uuid { get; set; } = Guid.NewGuid().ToString();
        public WithdrawalPaymentMethodsEnum? PaymentType { get; set; }
        public decimal FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalFees { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Currency Currency { get; set; }
        public PayoutSetting? PayoutSetting { get; set; }
        public WithdrawalDetail WithdrawalDetail { get; set; }
    }
}
