using AdminLte.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.Areas.User.Models
{
    public class DepositViewModel
    {
        public string UserId { get; set; }
        public string PaymentType { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public decimal Amount { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select payment method")]
        public int PaymentMethodId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Pleaseselect currency")]
        public int CurrencyId { get; set; }

        public decimal? FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal? TotalFees { get; set; }

        public bool EarlyPay { get; set; } = false;
        public List<Bank> Banks { get; set; }

        public int BankId { get; set; }
        public int PaymentTypeId { get; set; }

        public string Status { get; set; }
        public IFormFile File { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}
