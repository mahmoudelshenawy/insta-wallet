using AdminLte.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test.Faker
{
    internal class CurrencyFactory
    {
        public string[] Currencies = new string[] { "USD", "EUR", "EGP" };
        public List<Currency> GenerateCurrenciesList()
        {
            var currencies = new List<Currency>();
           
            for (int i = 0; i < Currencies.Length; i++)
            {
                var currency = new Currency
                {
                    Id = i + 1,
                    Code = Currencies[i],
                    Name = Currencies[i],
                    Status = StatusEnum.Active,
                    Default = i == 0 ? true : false,
                    Type = "Fyatu",
                    FeeLimits = new FeeLimitFaker().GenerateListOfFeesLimits().Where(c => c.CurrencyId == i + 1).ToList()
                };
                currencies.Add(currency);
            }
            
            return currencies;
        }
    }
}
