using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class StripFormViewModel
    {
        [Required]
        public string SecretKey { get; set; }
        [Required]
        public string PublishableKey { get; set; }
        [Required]
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
