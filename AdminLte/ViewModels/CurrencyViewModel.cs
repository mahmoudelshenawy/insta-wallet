using AdminLte.Data.Entities;
using AdminLte.Rules;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Symbol { get; set; }
        public string? Logo { get; set; }
        [Required]
        public IFormFile LogoFile { get; set; }
        public bool Default { get; set; }
        [Required(ErrorMessage ="Please Select Avalue")]
        public ExchangeFromEnum ExchangeFrom { get; set; }
        [Required(ErrorMessage = "Please Select Avalue")]
        public StatusEnum Status { get; set; }
    }

    //public enum StatusEnum
    //{
    //    Active,
    //    Iactive
    //}
  

    //public enum ExchangeFromEnum
    //{
    //    Local,
    //    Api
    //}
}
