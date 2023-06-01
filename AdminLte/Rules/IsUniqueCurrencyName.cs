using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.Rules
{
    public class IsUniqueCurrencyName : ValidationAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly string _tableName;
        public IsUniqueCurrencyName(ApplicationDbContext context, string tableName)
        {
            _context = context;
            _tableName = tableName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var attribute = validationContext.MemberName; //attribute - table - value - optional : ignoreSelf
            var model = validationContext.ObjectInstance;
            var modelType = model.GetType();
            var exists = _context.Currencies.FirstOrDefault(c => c.Name == (string) value);
         var cc =   _context.Entry(_tableName);

            if(exists != null)
            {
                return new ValidationResult("the name should be unique");
            }
           return ValidationResult.Success;
          //  return base.IsValid(value, validationContext);
        }
       
    }
}
