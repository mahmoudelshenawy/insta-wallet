using AdminLte.Areas.User.Models;
using Stripe;

namespace AdminLte.Areas.Repositories
{
    public interface IStripeRepository
    {
        PaymentIntent? DoStuffInStripe();

        Task<StripeViewModel> CreatePaymentAction(StripeViewModel stripeView);
        string CheckPaymentValidationAfterRedirectBack(string payment_intent, StripeViewModel stripeView);
    }
}