using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class PaymobFormViewModel
    {
        [Required]
        public string ApiKey { get; set; }
        [Required]
        public string IntegrationId { get; set; }
        [Required]
        public string IFrameId { get; set; }
        [Required]
        public string Hmac { get; set; }
        [Required]
        public string Status { get; set; }
        public string ActivatedFor { get; set; }
        [Required]
        public string Pm { get; set; }
        [Required]
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
