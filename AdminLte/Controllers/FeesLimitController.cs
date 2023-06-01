using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Controllers
{
    [Route("admin/Currencies")]
    public class FeesLimitController : Controller
    {
        private readonly ApplicationDbContext _context;

        private List<string> _depositMethods = new List<string>{"Strip","Stripe", "Paypal", "Paymob","Bank","PayUmoney",
            "PayeerWallet", "Bitcoin","VodafoneCash", "PerfectMoney", "Usdt", "Payoneer", "ZainCashIraq"};

        private List<string> _withdrawalMethods = new List<string>{"Paypal","Bank","Cash",
            "PayeerWallet", "Bitcoin","VodafoneCash", "PerfectMoney", "Usdt", "Payoneer", "ZainCashIraq"};
        public FeesLimitController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet("fees-limit/{id}/{tType?}")]
        public async Task<IActionResult> Index(int id, string tType)
        {
            ViewBag.subItem = "Currencies";
            var data = await GetBasicData(id, tType);
            return View(data);
        }
        [HttpPost("fees-limit/{id}/{tType?}")]
        public async Task<object> AddFeeLimit(FeesLimitFormViewModel feeLimitForm)
        {
            if (ModelState.IsValid)
            {
                if (feeLimitForm.TypeToDisplay == "Multiple")
                {
                    var result = await AddMultipleFeesLimit(feeLimitForm);
                    if (result != null)
                    {
                        ViewBag.IsSuccess = true;
                    }
                }
                else
                {
                    var result = await AddSingleFeesLimit(feeLimitForm);
                    if (result != null)
                    {
                        ViewBag.IsSuccess = true;
                    }
                }
            }
            var data = await GetBasicData(feeLimitForm.CurrencyId, feeLimitForm.tType);
            return View("Index", data);
        }

        private async Task<object> AddMultipleFeesLimit(FeesLimitFormViewModel feeLimitForm)
        {
            for (int i = 0; i < feeLimitForm.PaymentMethods.Count(); i++)
            {
                //CHECK IF feelimit exists to update or create
                var feeLimit = await _context.FeesLimits
                   .Where(fee =>
                   fee.TransactionTypeId == feeLimitForm.TransactionTypeId
                   && fee.CurrencyId == feeLimitForm.CurrencyId
                   && fee.PaymentMethodId == feeLimitForm.FeesLimits.PaymentMethodId[i]).FirstOrDefaultAsync();
                if (feeLimit != null)
                {
                    feeLimit.MinLimit = feeLimitForm.FeesLimits.MinLimit[i];
                    feeLimit.MaxLimit = feeLimitForm.FeesLimits.MaxLimit[i];
                    feeLimit.FixedAmount = feeLimitForm.FeesLimits.FixedAmount[i];
                    feeLimit.PercentAmount = feeLimitForm.FeesLimits.PercentAmount[i];
                    feeLimit.HasTransaction = feeLimitForm.FeesLimits.HasTransaction[i];
                    _context.Update(feeLimit);
                }
                else
                {
                    var newFeeLimit = new FeeLimit
                    {
                        CurrencyId = feeLimitForm.CurrencyId,
                        TransactionTypeId = feeLimitForm.TransactionTypeId,
                        PaymentMethodId = feeLimitForm.FeesLimits.PaymentMethodId[i],
                        MinLimit = feeLimitForm.FeesLimits.MinLimit[i],
                        MaxLimit = feeLimitForm.FeesLimits.MaxLimit[i],
                        FixedAmount = feeLimitForm.FeesLimits.FixedAmount[i],
                        PercentAmount = feeLimitForm.FeesLimits.PercentAmount[i],
                        HasTransaction = feeLimitForm.FeesLimits.HasTransaction[i],
                    };
                    _context.Add(newFeeLimit);

                }
                await _context.SaveChangesAsync();
            }
            var data = new
            {
                IsAdded = true
            };
            return data;
        }

        private async Task<object> AddSingleFeesLimit(FeesLimitFormViewModel feeLimitForm)
        {
            var feeLimit = await _context.FeesLimits
                   .Where(fee =>
                   fee.TransactionTypeId == feeLimitForm.TransactionTypeId
                   && fee.CurrencyId == feeLimitForm.CurrencyId
                   ).FirstOrDefaultAsync();

            if (feeLimit != null)
            {
                feeLimit.MinLimit = feeLimitForm.SingleForm.MinLimit;
                feeLimit.MaxLimit = feeLimitForm.SingleForm.MaxLimit;
                feeLimit.FixedAmount = feeLimitForm.SingleForm.FixedAmount;
                feeLimit.PercentAmount = feeLimitForm.SingleForm.PercentAmount;
                feeLimit.HasTransaction = feeLimitForm.SingleForm.HasTransaction;
                _context.Update(feeLimit);
            }
            else
            {
                var newFeeLimit = new FeeLimit
                {
                    CurrencyId = feeLimitForm.CurrencyId,
                    TransactionTypeId = feeLimitForm.TransactionTypeId,
                    MinLimit = feeLimitForm.SingleForm.MinLimit,
                    MaxLimit = feeLimitForm.SingleForm.MaxLimit,
                    FixedAmount = feeLimitForm.SingleForm.FixedAmount,
                    PercentAmount = feeLimitForm.SingleForm.PercentAmount,
                    HasTransaction = feeLimitForm.SingleForm.HasTransaction,
                };
                _context.Add(newFeeLimit);

            }
            await _context.SaveChangesAsync();
            var data = new
            {
                IsAdded = true
            };
            return data;
        }
        private async Task<FeesLimitFormViewModel> GetFeesLimitOfType(int currencyId, int transactionTypeId, TransactionTypeEnum transactionType)
        {
            var feesLimitViewModel = new FeesLimitFormViewModel
            {
                CurrencyId = currencyId,
                TransactionTypeId = transactionTypeId,
                TransactionType = transactionType.ToString()
            };
            switch (transactionType)
            {
                case TransactionTypeEnum.Deposit:

                    var paymentMethods = await _context.PaymentMethods
                        .Include(pm => pm.FeeLimits
                        .Where(fee => fee.TransactionTypeId == transactionTypeId && fee.CurrencyId == currencyId)
                        )
                        .Where(p => _depositMethods.Contains(p.Name))
                        .ToListAsync();
                    var PaymentMethodsIds = new List<int>();
                    var FeeFormLimit = new FeesLimitsForm
                    {
                        TransactionTypeId = transactionTypeId,
                        CurrencyId = currencyId,
                        PaymentMethodId = PaymentMethodsIds,
                        FixedAmount = new List<decimal>(),
                        PercentAmount = new List<decimal>(),
                        MinLimit = new List<decimal>(),
                        MaxLimit = new List<decimal?>(),
                        HasTransaction = new List<bool>(),


                    };
                    foreach (var paymentMethod in paymentMethods)
                    {
                        PaymentMethodsIds.Add(paymentMethod.Id);
                        FeeFormLimit.PaymentMethodId = PaymentMethodsIds;
                        if (paymentMethod.FeeLimits.Count() > 0)
                        {
                            FeeFormLimit.FixedAmount.Add(paymentMethod.FeeLimits[0].FixedAmount);
                            FeeFormLimit.PercentAmount.Add(paymentMethod.FeeLimits[0].PercentAmount);
                            FeeFormLimit.MinLimit.Add(paymentMethod.FeeLimits[0].MinLimit);
                            FeeFormLimit.MaxLimit.Add(paymentMethod.FeeLimits[0].MaxLimit);
                            FeeFormLimit.HasTransaction.Add(paymentMethod.FeeLimits[0].HasTransaction);
                        }
                        else
                        {
                            FeeFormLimit.FixedAmount.Add(0.00m);
                            FeeFormLimit.PercentAmount.Add(0.00m);
                            FeeFormLimit.MinLimit.Add(0.00m);
                            FeeFormLimit.MaxLimit.Add(null);
                            FeeFormLimit.HasTransaction.Add(false);
                        }
                    }
                    feesLimitViewModel.PaymentMethods = paymentMethods;
                    feesLimitViewModel.FeesLimits = FeeFormLimit;
                    feesLimitViewModel.TypeToDisplay = "Multiple";
                    break;
                case TransactionTypeEnum.Withdrawal:

                    var withdrawalPaymentMethods = await _context.PaymentMethods
                         .Include(pm => pm.FeeLimits
                        .Where(fee => fee.TransactionTypeId == transactionTypeId && fee.CurrencyId == currencyId))
                         .Where(p => _withdrawalMethods.Contains(p.Name))
                        .ToListAsync();
                    var withdrawalPaymentMethodsIds = new List<int>();
                    var withdrawalFeeFormLimit = new FeesLimitsForm
                    {
                        TransactionTypeId = transactionTypeId,
                        CurrencyId = currencyId,
                        PaymentMethodId = withdrawalPaymentMethodsIds,
                        FixedAmount = new List<decimal>(),
                        PercentAmount = new List<decimal>(),
                        MinLimit = new List<decimal>(),
                        MaxLimit = new List<decimal?>(),
                        HasTransaction = new List<bool>(),


                    };
                    foreach (var paymentMethod in withdrawalPaymentMethods)
                    {
                        withdrawalPaymentMethodsIds.Add(paymentMethod.Id);
                        withdrawalFeeFormLimit.PaymentMethodId = withdrawalPaymentMethodsIds;
                        if (paymentMethod.FeeLimits.Count() > 0)
                        {
                            withdrawalFeeFormLimit.FixedAmount.Add(paymentMethod.FeeLimits[0].FixedAmount);
                            withdrawalFeeFormLimit.PercentAmount.Add(paymentMethod.FeeLimits[0].PercentAmount);
                            withdrawalFeeFormLimit.MinLimit.Add(paymentMethod.FeeLimits[0].MinLimit);
                            withdrawalFeeFormLimit.MaxLimit.Add(paymentMethod.FeeLimits[0].MaxLimit);
                            withdrawalFeeFormLimit.HasTransaction.Add(paymentMethod.FeeLimits[0].HasTransaction);
                        }
                        else
                        {
                            withdrawalFeeFormLimit.FixedAmount.Add(0.00m);
                            withdrawalFeeFormLimit.PercentAmount.Add(0.00m);
                            withdrawalFeeFormLimit.MinLimit.Add(0.00m);
                            withdrawalFeeFormLimit.MaxLimit.Add(null);
                            withdrawalFeeFormLimit.HasTransaction.Add(false);
                        }
                    }
                    feesLimitViewModel.FeesLimits = withdrawalFeeFormLimit;
                    feesLimitViewModel.PaymentMethods = withdrawalPaymentMethods;
                    feesLimitViewModel.TypeToDisplay = "Multiple";
                    break;

                default:
                    var feesLimitSingle = await _context.FeesLimits
                        .Select(fe => new FeeLimitForm
                        {
                            CurrencyId = fe.CurrencyId,
                            TransactionTypeId = fe.TransactionTypeId,
                            FixedAmount = fe.FixedAmount,
                            HasTransaction = fe.HasTransaction,
                            MaxLimit = fe.MaxLimit,
                            MinLimit = fe.MinLimit,
                            PaymentMethodId = fe.PaymentMethodId,
                            PercentAmount = fe.PercentAmount,
                            ProcessingTime = fe.ProcessingTime
                        })
                        .Where(fe => fe.TransactionTypeId == transactionTypeId && fe.CurrencyId == currencyId)
                        .FirstOrDefaultAsync();
                    if (feesLimitSingle == null)
                    {
                        feesLimitSingle = new FeeLimitForm
                        {
                            CurrencyId = currencyId,
                            TransactionTypeId = transactionTypeId,
                            FixedAmount = 0.00m,
                            PercentAmount = 0.00m,
                            MinLimit = 0.00m,
                            MaxLimit = null,
                            HasTransaction = false,

                        };
                    }

                    feesLimitViewModel.SingleForm = feesLimitSingle;
                    feesLimitViewModel.TypeToDisplay = "Single";
                    break;
            }

            return feesLimitViewModel;
        }
        private async Task<object> GetBasicData(int id, string tType)
        {
            tType = !string.IsNullOrEmpty(tType) ? tType : "Deposit";
            string tab = !string.IsNullOrEmpty(tType) ? tType : "Deposit";

            if (tType == "Transfer")
            {
                tab = "Transferred";
            }
            else if (tType == "Exchange")
            {
                tab = "Exchange_From";
            }
            else if (tType == "RequestPayment")
            {
                tab = "Request_To";
            }

            var transactionType = await _context.TransactionTypes
                .Where(b => b.Name == Enum.Parse<TransactionTypeEnum>(tab)).FirstOrDefaultAsync();

            string TransactionTypeName = "";
            if (transactionType != null)
            {
                TransactionTypeName = tType.Contains("_") ? tType.Replace('_', ' ') : tType;
            }
            var currentCurrency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);

            var currencies = await _context.Currencies.Select(x =>
         new
         {
             Id = x.Id,
             Name = x.Name
         }).ToListAsync();

            var data = new
            {
                tType = tType,
                currentCurrency = currentCurrency,
                Currencies = currencies,
                TransactionTypeName = TransactionTypeName,
                FeesLimitData = await GetFeesLimitOfType(id, transactionType.Id, transactionType.Name)
            };
            return data;
        }

    }
}
