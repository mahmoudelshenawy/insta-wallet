using AdminLte.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test.Faker
{
    internal class WalletFaker
    {
        public List<Wallet> GenerateWallets()
        {
            var wallets = new List<Wallet>();
            var userId = new ApplicationUserFaker().GenerateApplicationUser().First().Id;
            wallets.Add(new Wallet
            {
                Id = 1,
                CurrencyId = 1,
                IsDefault = true,
                UserId = userId,
                Balance = 100000m,
                Currency = new CurrencyFactory().GenerateCurrenciesList().FirstOrDefault(c => c.Id == 1)
            });
            wallets.Add(new Wallet
            {
                Id = 2,
                CurrencyId = 2,
                UserId = userId,
                Balance = 4000m,
                Currency = new CurrencyFactory().GenerateCurrenciesList().FirstOrDefault(c => c.Id == 2)
            });
            wallets.Add(new Wallet
            {
                Id = 2,
                CurrencyId = 3,
                UserId = userId,
                Balance = 6000m,
                Currency = new CurrencyFactory().GenerateCurrenciesList().FirstOrDefault(c => c.Id == 3)
            });
            return wallets;
        }
    }
}
