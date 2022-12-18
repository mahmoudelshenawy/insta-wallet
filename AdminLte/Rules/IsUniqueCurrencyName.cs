using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.Rules
{
    public class IsUniqueCurrencyName : ValidationAttribute
    {
        private readonly ApplicationDbContext _context;
        public IsUniqueCurrencyName(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (CurrencyViewModel)validationContext.ObjectInstance;
            var exists = _context.Currencies.FirstOrDefault(c => c.Name == (string) value);
            if(exists != null)
            {
                return new ValidationResult("the name should be unique");
            }
           return ValidationResult.Success;
          //  return base.IsValid(value, validationContext);
        }
       
    }
}
