using AdminLte.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class PaymentMethodViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public PaymentMethodStatusEnum Status { get; set; }

        public FeeLimitForm? FeeLimit { get; set; }
    }
}
