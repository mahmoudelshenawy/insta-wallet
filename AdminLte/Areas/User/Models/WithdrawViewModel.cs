using AdminLte.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.Areas.User.Models
{
    public class WithdrawViewModel
    {
        public int? Id { get; set; }
        public IEnumerable<SelectListItem> Currencies { get; set; }
        public IEnumerable<SelectListItem> PaymentMethods { get; set; }
        [Required]
        public int PaymentMethodId { get; set; }
        [Required]
        public int CurrencyId { get; set; }
        [Required]

        public string? PaymentMethod { get; set; }
        public string? Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal? FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal? TotalFees { get; set; }
        public decimal? TotalAmount { get; set; }
        public PayoutSettingViewModel PayoutSetting { get; set; }

        public WithdrawViewModel()
        {
            PayoutSetting = new PayoutSettingViewModel();
        }
    }
}
