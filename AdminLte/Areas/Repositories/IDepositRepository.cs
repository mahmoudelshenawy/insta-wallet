using AdminLte.Areas.User.Models;
using AdminLte.Data.Entities;

namespace AdminLte.Areas.Repositories
{
    public interface IDepositRepository
    {
        Task<List<Currency>> ActiveCurrencyListForDeposit();
        Task<List<PaymentMethod>> ActivePaymentMethodsForSelectedCurrency(int currency_id);
        Task<FeeLimit> getFeeLimitOfOneMethod(int currencyId, int paymentMethodId);
        Task<Wallet?> getUserBalance(string uid, int currencyId);
        Task<PaymentMethod?> getPaymentMethod(int paymentMethodId);

        Task<List<Bank>> GetBankList(int paymentMethodId, int currencyId);
         Task<Bank?> GetBankDetails(int bank_id);

        Task<bool> CreateDepositSuccess(DepositViewModel depositView);
    }
}