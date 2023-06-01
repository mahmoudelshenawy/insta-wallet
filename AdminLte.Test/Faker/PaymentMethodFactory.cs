using AdminLte.Data.Entities;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test.Faker
{
    public class PaymentMethodFactory
    {
        string[] MethodsNames = new string[] {"Bank","Cash","VodafoneCash","ZainCashIraq","PayeerWallet","Bitcoin",
            "PerfectMoney", "Usdt","Payoneer",  "OrangeMoney" ,"Stripe","Paymob", "Paypal"};
        public List<PaymentMethod> GeneratePaymentMethodsList()
        {

            List<PaymentMethod> paymentMethods = new();
            for (int i = 0; i < MethodsNames.Length; i++)
            {
                var paymentMethod = new PaymentMethod
                {
                    Id = i + 1,
                    Name = MethodsNames[i],
                    Status = Data.Enums.PaymentMethodStatusEnum.Active
                };
                paymentMethods.Add(paymentMethod);
            }
            return paymentMethods;
        }
    }
}
