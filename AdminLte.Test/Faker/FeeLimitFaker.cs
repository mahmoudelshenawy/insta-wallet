using AdminLte.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test.Faker
{
    internal class FeeLimitFaker
    {
        public List<FeeLimit> GenerateListOfFeesLimits()
        {
            var feesLimit = new List<FeeLimit>();
            for (int i = 1; i < 4; i++)
            {
                /* Withdrawal */
                feesLimit.Add(new FeeLimit //Bank 
                {
                    Id = 1 * i,
                    CurrencyId = i,
                    PaymentMethodId = 1,
                    TransactionTypeId = 2,
                    HasTransaction = true,
                    MinLimit = 100,
                    MaxLimit = 10000,
                    FixedAmount = 0.00m,
                    PercentAmount = 1
                });
                feesLimit.Add(new FeeLimit //cash
                {
                    Id = 2* i,
                    CurrencyId = i,
                    PaymentMethodId = 2,
                    TransactionTypeId = 2,
                    HasTransaction = true,
                    MinLimit = 100,
                    MaxLimit = 10000,
                    FixedAmount = 0.00m,
                    PercentAmount = 1
                });
                feesLimit.Add(new FeeLimit //vodafoneCash
                {
                    Id = 3 * i,
                    CurrencyId = i,
                    PaymentMethodId = 3,
                    TransactionTypeId = 2,
                    HasTransaction = true,
                    MinLimit = 100,
                    MaxLimit = 10000,
                    FixedAmount = 0.00m,
                    PercentAmount = 1
                });
            }
           

            return feesLimit;
        }
    }
}
