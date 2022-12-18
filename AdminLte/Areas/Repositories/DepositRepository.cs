using AdminLte.Areas.User.Models;
using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AdminLte.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminLte.Areas.Repositories
{
    public class DepositRepository : IDepositRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUploadService _UploadService;

        public DepositRepository(ApplicationDbContext context, IUploadService uploadService)
        {
            _context = context;
            _UploadService = uploadService;
        }

        public async Task<List<Currency>> ActiveCurrencyListForDeposit()
        {

            var activeCurrencies = await _context.Currencies.Select(c => new Currency
            {
                Id = c.Id,
                Code = c.Code,
                Status = c.Status
            }).Where(c => c.Status == StatusEnum.Active).ToListAsync();

            var feesLimits = await _context.FeesLimits.Where(f => f.TransactionTypes.Name == Data.Enums.TransactionTypeEnum.Deposit &&
            f.HasTransaction == true)
                .ToListAsync();
            var selectedCurrencies = new List<Currency>();
            foreach (var currency in activeCurrencies)
            {
                foreach (var feeLimit in feesLimits)
                {
                    if (currency.Id == feeLimit.CurrencyId && currency.Status == StatusEnum.Active && feeLimit.HasTransaction == true)
                    {
                        if (selectedCurrencies.Where(c => c.Id == currency.Id).Count() == 0)
                        {
                            selectedCurrencies.Add(currency);
                        }

                    }
                }
            }
            return selectedCurrencies;
        }

        public async Task<List<PaymentMethod>> ActivePaymentMethodsForSelectedCurrency(int currency_id)
        {
            var paymentMethods = new List<PaymentMethod>();
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            var feesLimit = await _context.FeesLimits
                .Include(f => f.PaymentMethod)
                   .Where(fe => fe.TransactionTypes.Name == TransactionTypeEnum.Deposit &&
            fe.CurrencyId == currency_id && fe.HasTransaction == true)
                .ToListAsync();

            var currencyPaymentMethods = await _context.CurrencyPaymentMethods.Where(c => c.CurrencyId == currency_id 
            && c.ActivatedFor.Contains("Deposit"))
                .ToListAsync();
            var depositCurrencyPaymentMethods = new List<CurrencyPaymentMethod>();
            foreach (var cpm in currencyPaymentMethods)
            {
                var activated_For = JsonSerializer.Deserialize<string[]>(cpm.ActivatedFor, options);
                if (activated_For.Count() > 0 && activated_For.Any(c => c == "Deposit"))
                {
                    depositCurrencyPaymentMethods.Add(cpm);
                }

            }

            foreach (var feelimit in feesLimit)
            {
                foreach (var cpm in currencyPaymentMethods)
                {
                    if (feelimit.PaymentMethodId == cpm.PaymentMethodId)
                    {
                        if (!paymentMethods.Any(p => p.Id == feelimit.PaymentMethodId))
                        {
                            paymentMethods.Add(feelimit.PaymentMethod);
                        }
                    }
                }
            }
            return paymentMethods;
        }

        public async Task<FeeLimit> getFeeLimitOfOneMethod(int currencyId, int paymentMethodId)
        {
            var feeLimit = await _context.FeesLimits
                .Where(f => f.CurrencyId == currencyId
                && f.PaymentMethodId == paymentMethodId
                && f.TransactionTypes.Name == TransactionTypeEnum.Deposit
                ).FirstOrDefaultAsync();
            return feeLimit;
        }

        public async Task<Wallet?> getUserBalance(string uid, int currencyId)
        {
            return await _context.Wallets.Where(w => w.UserId == uid && w.CurrencyId == currencyId).FirstOrDefaultAsync();
        }
        public async Task<PaymentMethod?> getPaymentMethod(int paymentMethodId)
        {
            return await _context.PaymentMethods.Where(p => p.Id == paymentMethodId).FirstOrDefaultAsync();
        }

        public async Task<List<Bank>> GetBankList(int paymentMethodId, int currencyId)
        {
            var banks = await _context.Banks.Where(b => b.CurrencyId == currencyId && b.Active == true).ToListAsync();
            var banksList = new List<Bank>();
            var currencyPaymentMethods = await _context.CurrencyPaymentMethods.Where(cpm => cpm.CurrencyId == currencyId
            && cpm.PaymentMethodId == paymentMethodId && cpm.ActivatedFor.Contains("Deposit"))
                .Select(cpm => new CurrencyPaymentMethod()
                {
                    MethodData = cpm.MethodData
                }).ToListAsync();

            foreach (var bank in banks)
            {
                foreach (var cpm in currencyPaymentMethods)
                {
                    var bank_id = (int?) JObject.Parse(cpm.MethodData)["bankId"];
                    if(bank.Id == bank_id)
                    {
                        banksList.Add(bank);
                    }
                }
            }

            return banksList;

        }
        public async Task<Bank?> GetBankDetails(int bank_id)
        {
            return await _context.Banks.Where(b => b.Id == bank_id).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateDepositSuccess(DepositViewModel depositView)
        {
            var FileId = 0;
            var transactionType = await _context.TransactionTypes
              .Where(b => b.Name == TransactionTypeEnum.Deposit).FirstOrDefaultAsync();
            if (depositView.File != null)
            {
                var file = new Attachment
                {
                    Path = await _UploadService.UploadImage("uploads/files/deposits/", depositView.File),
                    Type = depositView.File.ContentType
                };
                _context.Add(file);
              await  _context.SaveChangesAsync();
                FileId = file.Id;
            }
            var deposit = new Deposit
            {
                UserId = depositView.UserId,
                CurrencyId = depositView.CurrencyId,
                PaymentMethodId = depositView.PaymentMethodId,
                FixedFeeAmount = depositView.FixedFeeAmount ?? 0.00m,
                PercentFeeAmount = depositView.PercentFeeAmount,
                Amount = depositView.Amount,
                PaymentTypeId = depositView.PaymentTypeId,
                Uuid = GenerateSecurityStamp(),
                Status = Enum.Parse<PaymentStatus>(depositView.Status),
                PaymentType = Enum.Parse<PaymentTypeEnum>(depositView.PaymentType),
                AttachmentId = FileId,
            };
           
             _context.Deposits.Add(deposit);
            await _context.SaveChangesAsync();
            var transaction = new Transaction
            {
                UserId = depositView.UserId,
                CurrencyId = depositView.CurrencyId,
                PaymentMethodId = depositView.PaymentMethodId,
                FixedFeeAmount = depositView.FixedFeeAmount ?? 0.00m,
                PercentFeeAmount = depositView.PercentFeeAmount,
                Uuid = GenerateSecurityStamp(),
                Status = Enum.Parse<PaymentStatus>(depositView.Status),
                PaymentType = Enum.Parse<PaymentTypeEnum>(depositView.PaymentType),
                Total = depositView.Amount,
                Subtotal = depositView.Amount  - (depositView.TotalFees ?? 0.00m),
                TransactionReferenceId = deposit.Id,
                TransactionTypeId = transactionType.Id,
                PaymentTypeId = depositView.PaymentTypeId,
                UserType = !string.IsNullOrEmpty(depositView.UserId) ? UserRegisterType.Registered : UserRegisterType.Unregistered, 
                AttachmentId = FileId
            };
            await _context.SaveChangesAsync();
            //based on paymentType add to wallet or not
            if (depositView.EarlyPay == true)
            {
                var wallet = _context.Wallets.Where(w => w.UserId == depositView.UserId && w.CurrencyId == depositView.CurrencyId).FirstOrDefault();

                wallet.Balance = wallet.Balance + (depositView.Amount - (depositView.TotalFees ?? 0.00m));
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        private string GenerateSecurityStamp()
        {
            byte[] bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
