using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminLte.ViewModels
{
    public class CurrencyPaymentMethodViewModel
    {
        public StripFormViewModel? StripFormViewModel { get; set; }
        public PaypalFormViewModel? PaypalFormViewModel { get; set; }
        public PaymobFormViewModel? PaymobFormViewModel { get; set; }
        public List<BankFormViewModel> BankFormViewModels { get; set; }
        public string? Pm { get; set; }

        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }

        public List<SelectListItem> Countries { get; set; }
    }

   
}
