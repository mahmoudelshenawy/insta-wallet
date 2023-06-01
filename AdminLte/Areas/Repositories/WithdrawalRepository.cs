using AdminLte.Areas.User.Models;
using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AdminLte.DataTableViewModels;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace AdminLte.Areas.Repositories
{
    public class WithdrawalRepository : IWithdrawalRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public WithdrawalRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethods()
        {
            var methods = Enum.GetNames(typeof(WithdrawalPaymentMethodsEnum)).ToList();
            return await _context.PaymentMethods.Where(pm => methods.Contains(pm.Name)).ToListAsync();
            //return await Task.FromResult(_context.PaymentMethods.Where(pm => methods.Contains(pm.Name)).ToList());
        }
        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            return await _context.Currencies.ToListAsync();
        }

        public async Task<List<Currency>> GetActiveCurrenciesForSelectedPaymentMethod(int PaymentMethodId, string userId)
        {
            var transactionTypeId = await _context.TransactionTypes.FirstOrDefaultAsync(t => t.Name == TransactionTypeEnum.Withdrawal);
            var currencies = new List<Currency>();
            //check if user has wallet in this currency
            //feesLimits of this method
            var wallets = await _context.Wallets
                .Where(w => w.UserId == userId)
                 .Include(w => w.Currency.FeeLimits.Where(c => c.TransactionTypeId == transactionTypeId.Id &&
                 c.PaymentMethodId == PaymentMethodId
                 && c.HasTransaction)
                 )
                .ToListAsync();

            foreach (var wallet in wallets)
            {
                if (wallet.Currency.FeeLimits.Any(f => f.CurrencyId == wallet.CurrencyId))
                {
                    currencies.Add(new Currency
                    {
                        Id = wallet.Currency.Id,
                        Name = wallet.Currency.Name,
                        Code = wallet.Currency.Code,
                        Default = wallet.Currency.Default
                    });
                }
            }
            return currencies;
        }

        public async Task<List<Country>> GetCountriesList()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<SuccessModel> GetFeesLimitAndCheckBalanceAvailability(string body, string userId)
        {
            var amount = (decimal)JObject.Parse(body)["amount"];
            var currencyId = (int)JObject.Parse(body)["currencyId"];
            var paymentMethodId = (int)JObject.Parse(body)["paymentMethodId"];

            var successModel = new SuccessModel();
            successModel.DataSet = new Dictionary<string, string>();
            if (currencyId == 0 || paymentMethodId == 0 || amount == 0)
            {
                successModel.Message = "Data is not valid";
            }
            var feeLimit = await _context.FeesLimits.Include(f => f.Currency).FirstOrDefaultAsync(f => f.PaymentMethodId == paymentMethodId
            && f.CurrencyId == currencyId && f.HasTransaction);
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId && w.CurrencyId == currencyId);
            if (feeLimit == null)
            {
                successModel.Message = "this method is not available for this currency";
            }
            else
            {
                successModel.DataSet.Add("FixedFeeAmount", feeLimit.FixedAmount.ToString() + " " + feeLimit.Currency.Code);
                successModel.DataSet.Add("PercentFeeAmount", feeLimit.PercentAmount.ToString());
                successModel.DataSet.Add("MinLimit", feeLimit.MinLimit.ToString() + " " + feeLimit.Currency.Code);
                successModel.DataSet.Add("MaxLimit", feeLimit.MaxLimit.ToString() + " " + feeLimit.Currency.Code);
            }
            if (wallet == null)
            {
                successModel.Message = "you currently do not have a wallet for this currency";
            }

            if (amount < feeLimit.MinLimit)
            {
                successModel.Message = "amount is less than the minimum amount allowed" + feeLimit.MinLimit;
            }

            if (amount > feeLimit.MaxLimit)
            {
                successModel.Message = "amount is higher than the maximum amount allowed" + feeLimit.MaxLimit;
            }

            //calculate fees amount
            var totalFees = (amount * feeLimit.PercentAmount / 100) + feeLimit.FixedAmount;
            successModel.DataSet.Add("TotalFees", totalFees.ToString() + " " + feeLimit.Currency.Code);
            if (wallet.Balance < amount + totalFees)
            {
                successModel.Message = "you dont have enough balance!";
            }
            else
            {
                successModel.Success = true;
                successModel.DataSet.Add("totalAmount", (amount - totalFees).ToString());
            }

            return successModel;
        }

        public async Task<SuccessModel> CheckBalanceAvailability(WithdrawViewModel model, string userId)
        {
            var successModel = new SuccessModel();
            successModel.DataSet = new Dictionary<string, string>();
            successModel.DataSetNumbers = new Dictionary<string, decimal>();

            if (!model.PayoutSetting.NewPayoutSetting && model.PayoutSetting.PayoutSettingId == null)
            {
                successModel.Message = "please specify your payout out setting!";
                return successModel;
            }

            var feeLimit = await _context.FeesLimits
                .Include(f => f.Currency)
                .Include(fe => fe.PaymentMethod)
                .FirstOrDefaultAsync(f => f.PaymentMethodId == model.PaymentMethodId
           && f.CurrencyId == model.CurrencyId && f.HasTransaction);

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId && w.CurrencyId == model.CurrencyId);
            if (feeLimit == null)
            {
                successModel.Message = "this method is not available for this currency";
            }
            else
            {
                successModel.DataSet.Add("FixedFeeAmount", feeLimit.FixedAmount.ToString() + " " + feeLimit.Currency.Code);
                successModel.DataSet.Add("PercentFeeAmount", feeLimit.PercentAmount.ToString());
                successModel.DataSet.Add("MinLimit", feeLimit.MinLimit.ToString() + " " + feeLimit.Currency.Code);

                successModel.DataSet.Add("Currency", feeLimit.Currency.Code);
                successModel.DataSet.Add("PaymentMethod", feeLimit.PaymentMethod.Name);

                if (feeLimit.MaxLimit != null) successModel.DataSet.Add("MaxLimit", feeLimit.MaxLimit.ToString() + " " + feeLimit.Currency.Code);
            }
            if (wallet == null)
            {
                successModel.Message = "you currently do not have a wallet for this currency";
            }

            if (model.Amount < feeLimit.MinLimit)
            {
                successModel.Message = "amount is less than the minimum amount allowed " + feeLimit.MinLimit + " " + feeLimit.Currency.Code;
            }

            if (model.Amount > feeLimit.MaxLimit)
            {
                successModel.Message = "amount is higher than the maximum amount allowed " + feeLimit.MaxLimit + " " + feeLimit.Currency.Code;
            }

            //calculate fees amount
            var totalFees = (model.Amount * feeLimit.PercentAmount / 100) + feeLimit.FixedAmount;
            successModel.DataSet.Add("TotalFees", totalFees.ToString() + " " + feeLimit.Currency.Code);
            if (wallet.Balance < model.Amount + totalFees)
            {
                successModel.Message = "you dont have enough balance!";
            }
            else
            {
                successModel.Success = true;
                successModel.DataSet.Add("TotalAmount", (model.Amount - totalFees).ToString() + " " + feeLimit.Currency.Code);
                successModel.DataSet.Add("Amount", model.Amount.ToString() + " " + feeLimit.Currency.Code);

                successModel.DataSetNumbers.Add("TotalAmount", model.Amount - totalFees);
                successModel.DataSetNumbers.Add("TotalFees", totalFees);
                successModel.DataSetNumbers.Add("FixedFeeAmount", feeLimit.FixedAmount);
                successModel.DataSetNumbers.Add("PercentFeeAmount", feeLimit.PercentAmount);
            }
            return successModel;
        }
        public async Task<List<Currency>> GetCurrenciesList(int PaymentMethodId, string userId)
        {
            //'093732b4-e94b-40b0-a0ee-8dab1192aaba'
            var transactionTypeId = await Task.FromResult(_context.TransactionTypes.FirstOrDefault(t => t.Name == TransactionTypeEnum.Withdrawal));

            List<Currency> currencies = _context.Currencies.FromSqlInterpolated<Currency>(@$"
            SELECT c.* FROM Currencies AS c INNER JOIN Wallets AS w
                ON w.CurrencyId = c.Id INNER JOIN FeesLimits AS fe ON fe.CurrencyId = c.Id
                WHERE w.UserId = '{userId}'
                AND fe.TransactionTypeId = {transactionTypeId.Id} AND fe.PaymentMethodId = {PaymentMethodId};").ToList();

            var CurrenciesList = new List<Currency>();
            foreach (var c in currencies)
            {
                var currency = new Currency
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Default = c.Default,
                    Status = c.Status,
                    Type = c.Type
                };
                CurrenciesList.Add(currency);
            }
            return CurrenciesList;
        }
        public async Task<SuccessModel> CheckBalanceAvailabilityTest2(string userId, params decimal[] values)
        {
            var successModel = new SuccessModel();
            return successModel;
        }

        public async Task<bool> SubmitWithdrawalAndCreateTransaction(WithdrawViewModel model, string userId)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var response = await CheckBalanceAvailability(model, userId);
                var transactionType = await _context.TransactionTypes.FirstOrDefaultAsync(t => t.Name == TransactionTypeEnum.Withdrawal);
                if (!response.Success)
                {
                    return false;
                }
                model.FixedFeeAmount = response.DataSetNumbers.FirstOrDefault(x => x.Key == "FixedFeeAmount").Value;
                model.PercentFeeAmount = response.DataSetNumbers.FirstOrDefault(x => x.Key == "PercentFeeAmount").Value;
                model.TotalFees = response.DataSetNumbers.FirstOrDefault(x => x.Key == "TotalFees").Value;
                model.TotalAmount = response.DataSetNumbers.FirstOrDefault(x => x.Key == "TotalAmount").Value;
                var withdrawal = new Withdrawal
                {
                    UserId = userId,
                    Amount = model.Amount,
                    TotalFees = (decimal)model.TotalFees,
                    CurrencyId = model.CurrencyId,
                    FixedFeeAmount = (decimal)model.FixedFeeAmount,
                    PercentFeeAmount = model.PercentFeeAmount,
                    PaymentMethodId = model.PaymentMethodId,
                    Status = PaymentStatus.Success,
                    Uuid = new Guid().ToString(),
                    PaymentType = Enum.Parse<WithdrawalPaymentMethodsEnum>(model.PaymentMethod),
                };

                await _context.Withdrawals.AddAsync(withdrawal);
                await _context.SaveChangesAsync();
                var withdrawalDetails = new WithdrawalDetail();
                withdrawalDetails.WithdrawalId = withdrawal.Id;

                await SaveWithdrawalDetails(model, withdrawalDetails, withdrawal);

                await _context.WithdrawalDetails.AddAsync(withdrawalDetails);
                await _context.SaveChangesAsync();

                var transaction = new Transaction
                {
                    UserId = userId,
                    PercentFeeAmount = model.PercentFeeAmount,
                    FixedFeeAmount = (decimal)model.FixedFeeAmount,
                    CurrencyId = model.CurrencyId,
                    PaymentMethodId = model.PaymentMethodId,
                    Status = PaymentStatus.Success,
                    Subtotal = model.Amount,
                    Total = (decimal)model.TotalAmount,
                    TransactionReferenceId = withdrawal.Id,
                    UserType = UserRegisterType.Registered,
                    TransactionTypeId = transactionType.Id,
                    Uuid = Guid.NewGuid().ToString()
                };

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId && w.CurrencyId == model.CurrencyId);
                if (wallet != null)
                {
                    wallet.Balance = wallet.Balance - (model.TotalAmount ?? 0.00m);
                    _context.Wallets.Update(wallet);
                }

                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                //Notify User ==> [sending email - sending sms - realtime notification]
                return true;
            }
            catch (Exception e)
            {
                await _context.Database.RollbackTransactionAsync();
                return false;
            }
        }
        public async Task<List<PayoutSettingViewModel>> GetPayoutSettings(string userId, int paymentMethodId)
        {
            var payoutSettings = await _context.PayoutSettings
                  .Where(p => p.UserId == userId && p.PaymentMethodId == paymentMethodId)
                  .ToListAsync();
            var data = _mapper.Map<List<PayoutSettingViewModel>>(payoutSettings);
            return data;
        }
        public async Task<int> CheckIfNameExists()
        {
            var tableName = new SqlParameter("PaymentMethods", System.Data.SqlDbType.VarChar);
            var ColumnName = new SqlParameter("Name", System.Data.SqlDbType.VarChar);
            var ColumnValue = new SqlParameter("Cash", System.Data.SqlDbType.VarChar);
            var db = _context.Database.ExecuteSqlRaw(@$"IF EXISTS (SELECT * FROM {tableName} WHERE {ColumnName} LIKE '{ColumnValue}')
                                                      BEGIN
                                                      PRINT 'test';
                                                      END
                                                      ELSE
                                                      BEGIN
                                                      THROW 51000, 'The record does not exist.', 1;  
                                                      END");
            return db;
        }

        public async Task SaveWithdrawalDetails(WithdrawViewModel model, WithdrawalDetail withdrawalDetails, Withdrawal withdrawal)
        {
            if (model.PayoutSetting.SavePayoutSetting && model.PayoutSetting.PayoutSettingId == null)
            {
                //create new payout setting
                var payoutSetting = new PayoutSetting();
                if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Bank.ToString())
                {
                    payoutSetting.BankSetting = JsonSerializer.Serialize(model.PayoutSetting.BankSetting);
                    withdrawalDetails.BankSetting = JsonSerializer.Serialize(model.PayoutSetting.BankSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Cash.ToString())
                {
                    payoutSetting.CashSetting = JsonSerializer.Serialize(model.PayoutSetting.CashSetting);
                    withdrawalDetails.CashSetting = JsonSerializer.Serialize(model.PayoutSetting.CashSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Payoneer.ToString())
                {
                    payoutSetting.PayoneerSetting = JsonSerializer.Serialize(model.PayoutSetting.PayoneerSetting);
                    withdrawalDetails.PayoneerSetting = JsonSerializer.Serialize(model.PayoutSetting.PayoneerSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.VodafoneCash.ToString())
                {
                    payoutSetting.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.VodafoneCashSetting);
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.VodafoneCashSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.PayeerWallet.ToString())
                {
                    payoutSetting.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PayeerWalletSetting);
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PayeerWalletSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.PerfectMoney.ToString())
                {
                    payoutSetting.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PerfectMoneySetting);
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PerfectMoneySetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.OrangeMoney.ToString())
                {
                    payoutSetting.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.OrangeMoneySetting);
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.OrangeMoneySetting);
                }
                payoutSetting.PaymentMethodId = model.PaymentMethodId;
                payoutSetting.UserId = withdrawal.UserId;
                await _context.PayoutSettings.AddAsync(payoutSetting);
                await _context.SaveChangesAsync();
                withdrawal.PayoutSettingId = payoutSetting.Id;
            }
            else if (!model.PayoutSetting.SavePayoutSetting && model.PayoutSetting.PayoutSettingId == null)
            {
                if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Bank.ToString())
                {
                    withdrawalDetails.BankSetting = JsonSerializer.Serialize(model.PayoutSetting.BankSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Cash.ToString())
                {
                    withdrawalDetails.CashSetting = JsonSerializer.Serialize(model.PayoutSetting.CashSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Payoneer.ToString())
                {
                    withdrawalDetails.PayoneerSetting = JsonSerializer.Serialize(model.PayoutSetting.PayoneerSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.VodafoneCash.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.VodafoneCashSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.PayeerWallet.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PayeerWalletSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.PerfectMoney.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PerfectMoneySetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.OrangeMoney.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.OrangeMoneySetting);
                }
            }
            else
            {
                withdrawal.PayoutSettingId = model.PayoutSetting.PayoutSettingId;

                var payoutSetting = await _context.PayoutSettings.FirstOrDefaultAsync(p => p.Id == model.PayoutSetting.PayoutSettingId);

                if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Bank.ToString())
                {
                    withdrawalDetails.BankSetting = JsonSerializer.Serialize(model.PayoutSetting.BankSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Cash.ToString())
                {
                    withdrawalDetails.CashSetting = JsonSerializer.Serialize(model.PayoutSetting.CashSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.Payoneer.ToString())
                {
                    withdrawalDetails.PayoneerSetting = JsonSerializer.Serialize(model.PayoutSetting.PayoneerSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.VodafoneCash.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.VodafoneCashSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.PayeerWallet.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PayeerWalletSetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.PerfectMoney.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.PerfectMoneySetting);
                }
                else if (model.PaymentMethod == WithdrawalPaymentMethodsEnum.OrangeMoney.ToString())
                {
                    withdrawalDetails.WalletSetting = JsonSerializer.Serialize(model.PayoutSetting.OrangeMoneySetting);
                }

            }
        }
        public async Task<WithdrawViewModel> GetWithdrawalDetails()
        {
            var withdrawalModel = new WithdrawViewModel();

            var withdrawalData = await _context.Withdrawals
                .Include(w => w.WithdrawalDetail)
                .Include(w => w.PaymentMethod)
                .Include(w => w.Currency)
                .FirstOrDefaultAsync(w => w.Id == 3);

            //option A ==> AutoMapper
            var data = _mapper.Map<WithdrawViewModel>(withdrawalData);
            return data;

            //option B ==> manual mapping
            if (withdrawalData != null)
            {
                withdrawalModel.Amount = withdrawalData.Amount;
                withdrawalModel.TotalFees = withdrawalData.TotalFees;
                withdrawalModel.TotalAmount = withdrawalData.Amount - withdrawalData.TotalFees;
                withdrawalModel.CurrencyId = withdrawalData.CurrencyId;
                withdrawalModel.PaymentMethodId = withdrawalData.PaymentMethodId;
                withdrawalModel.FixedFeeAmount = withdrawalData.FixedFeeAmount;
                withdrawalModel.PercentFeeAmount = withdrawalData.PercentFeeAmount;
                withdrawalModel.PayoutSetting = new PayoutSettingViewModel();
                withdrawalModel.PayoutSetting.PayoutSettingId = withdrawalData.PayoutSettingId;
                if (withdrawalData.PaymentMethod.Name == WithdrawalPaymentMethodsEnum.Cash.ToString())
                {
                    withdrawalModel.PayoutSetting.CashSetting = JsonSerializer.Deserialize<CashSetting>(withdrawalData.WithdrawalDetail.CashSetting);
                }
            }
            return withdrawalModel;
        }
    }
}
