using AdminLte.Areas.User.Models;
using AdminLte.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class PayoutSetting
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? BankSetting { get; set; }
        public string? CashSetting { get; set; }
        public string? WalletSetting { get; set; }
        public string? PaypalSetting { get; set; }
        public string? PayoneerSetting { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
