using AdminLte.Areas.User.Models;
using AdminLte.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Stripe;

namespace AdminLte.Areas.Repositories
{
    public class StripeRepository : IStripeRepository
    {
        private readonly ApplicationDbContext _context;

        public StripeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //sk_test_51IOYAlK7OepXrE156Zn24XeRjSREC0OK3PJ2RuUeMENJhAXYNBJ4F58QK6GiWMApVRnpa0qOBhGE1OQqYUTFkTLf00I7TSDPyz
        //pk_test_51IOYAlK7OepXrE15zSuTELbKuxrLjyCILNjDjtU374pZdNcsHyy75sVzMKSfbpoRRiczE5jCf6x6hTx24HPSaK4v00dYkDrAlg

        //public StripeRepository()
        //{
        //    StripeConfiguration.ApiKey = "sk_test_51IOYAlK7OepXrE156Zn24XeRjSREC0OK3PJ2RuUeMENJhAXYNBJ4F58QK6GiWMApVRnpa0qOBhGE1OQqYUTFkTLf00I7TSDPyz";
        //}
        public PaymentIntent? DoStuffInStripe()
        {
            
            //regular card 4242424242424242
            //sca card 4000002500003155 -- 4000002760003184
            var PMoptions = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = "4000002500003155",
                    ExpMonth = 8,
                    ExpYear = 2023,
                    Cvc = "314",
                },
            };
            var PMservice = new PaymentMethodService();
            var paymentMethod = PMservice.Create(PMoptions);
            var paymentMethodId = paymentMethod.Id;

            var PIoptions = new PaymentIntentCreateOptions
            {
                Amount = 2000,
                Currency = "usd",
                PaymentMethodTypes = new List<string>
                       {
                       "card",
                        },
            };
            var PIservice = new PaymentIntentService();
            var paymentIntent = PIservice.Create(PIoptions);


            var PIConfirmoptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = paymentMethodId,
                ReturnUrl = "http://localhost:49432/deposit/stripe_payment"
            };
            var result = PIservice.Confirm(paymentIntent.Id, PIConfirmoptions);

            return result;
        }

        public async Task <StripeViewModel> CreatePaymentAction(StripeViewModel stripeView)
        {
            try
            {
                var currencyPaymentMethod = await _context.CurrencyPaymentMethods
                    .Where(cp => cp.CurrencyId == stripeView.CurrencyId && cp.PaymentMethodId == stripeView.PaymentMethodId)
                    .FirstOrDefaultAsync();

                var SecretKey = (string)JObject.Parse(currencyPaymentMethod.MethodData)["SecretKey"] ?? "";
                StripeConfiguration.ApiKey = SecretKey.Trim();
                var PMoptions = new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Number = stripeView.CardNumber,
                        ExpMonth = stripeView.Month,
                        ExpYear = stripeView.Year,
                        Cvc = stripeView.Cvc,
                    },
                    BillingDetails = new PaymentMethodBillingDetailsOptions
                    {
                        Name = stripeView.FullName,
                        Email = stripeView.Email,
                        Phone = stripeView.Phone
                    }
                };
                var PMservice = new PaymentMethodService();
                var paymentMethod = PMservice.Create(PMoptions);
                var paymentMethodId = paymentMethod.Id;

                var PIoptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)(stripeView.Amount * 100),
                    Currency = stripeView.Currency,
                    PaymentMethodTypes = new List<string>
                       {
                       "card",
                        },
                };
                var PIservice = new PaymentIntentService();
                var paymentIntent = PIservice.Create(PIoptions);


                var PIConfirmoptions = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethodId,
                    ReturnUrl = "http://localhost:49432/deposit/visa/callback"
                };

                var result = PIservice.Confirm(paymentIntent.Id, PIConfirmoptions);
                if(result.Status == "succeeded")
                {
                    stripeView.PaymentIntentId = result.Id;
                }else if(result.Status == "requires_payment_method") {
                    stripeView.Error = true;
                    stripeView.Message = "Payment failed";
                    stripeView.PaymentIntentId = result.Id;
                }
                else if(result.Status == "requires_action" && result.NextAction != null) {
                    var url = result.NextAction.RedirectToUrl.Url;
                    stripeView.RedirectToStripe = url;
                }
                return stripeView;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string CheckPaymentValidationAfterRedirectBack(string payment_intent , StripeViewModel stripeView)
        {
            var currencyPaymentMethod = _context.CurrencyPaymentMethods
                   .Where(cp => cp.CurrencyId == stripeView.CurrencyId && cp.PaymentMethodId == stripeView.PaymentMethodId).FirstOrDefault();

            var SecretKey = (string)JObject.Parse(currencyPaymentMethod.MethodData)["SecretKey"] ?? "";
            var service = new PaymentIntentService();
           var paymentIntent = service.Get(payment_intent);

           return paymentIntent.Status;
        }
    }
}
