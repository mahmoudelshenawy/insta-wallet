using Stripe;

namespace AdminLte.Areas.Repositories
{
    public class StripeRepository : IStripeRepository
    {
        //sk_test_51IOYAlK7OepXrE156Zn24XeRjSREC0OK3PJ2RuUeMENJhAXYNBJ4F58QK6GiWMApVRnpa0qOBhGE1OQqYUTFkTLf00I7TSDPyz
        //pk_test_51IOYAlK7OepXrE15zSuTELbKuxrLjyCILNjDjtU374pZdNcsHyy75sVzMKSfbpoRRiczE5jCf6x6hTx24HPSaK4v00dYkDrAlg
        public PaymentMethod? DoStuffInStripe()
        {
            StripeConfiguration.ApiKey = "sk_test_51IOYAlK7OepXrE156Zn24XeRjSREC0OK3PJ2RuUeMENJhAXYNBJ4F58QK6GiWMApVRnpa0qOBhGE1OQqYUTFkTLf00I7TSDPyz";
            var options = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = "4242424242424242",
                    ExpMonth = 8,
                    ExpYear = 2023,
                    Cvc = "314",
                },
            };
            var service = new PaymentMethodService();
           var result = service.Create(options);
            return result;
        }
    }
}
