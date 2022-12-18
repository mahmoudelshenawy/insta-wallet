using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class BankFormViewModel
    {
        public int BankId { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string BankBranchName { get; set; }
        public string BankBranchCity { get; set; }
        public string BankBranchAddress { get; set; }
        public string AccountName { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string SwiftCode { get; set; }
        public bool IsDefault { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
        public string Pm { get; set; }
        public IFormFile Logo { get; set; }
        public int CountryId { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public int AttachmentId { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }
    }
}
