using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class PaypalFormViewModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        public string Mode { get; set; }

        public int ProcessingTime { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string[] ActivatedFor { get; set; }

        [Required]
        public string Pm { get; set; }
        [Required]
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
