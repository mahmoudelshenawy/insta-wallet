using AdminLte.Areas.User.Models;
using AdminLte.Data.Entities;

namespace AdminLte.Areas.Repositories
{
    public interface IWithdrawalRepository
    {
        Task<List<PaymentMethod>> GetPaymentMethods();
        Task<IEnumerable<Currency>> GetCurrencies();

        Task<List<Currency>> GetActiveCurrenciesForSelectedPaymentMethod(int PaymentMethodId, string userId);

        Task<List<Country>> GetCountriesList();

        Task<SuccessModel> GetFeesLimitAndCheckBalanceAvailability(string body, string userId);

        Task<SuccessModel> CheckBalanceAvailability(WithdrawViewModel model, string userId);

        Task<List<Currency>> GetCurrenciesList(int PaymentMethodId, string userId);

        Task<bool> SubmitWithdrawalAndCreateTransaction(WithdrawViewModel model, string userId);

        Task<WithdrawViewModel> GetWithdrawalDetails();

        Task<List<PayoutSettingViewModel>> GetPayoutSettings(string userId, int paymentMethodId);
    }
}