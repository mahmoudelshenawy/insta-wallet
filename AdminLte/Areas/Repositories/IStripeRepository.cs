using Stripe;

namespace AdminLte.Areas.Repositories
{
    public interface IStripeRepository
    {
        PaymentMethod? DoStuffInStripe();
    }
}