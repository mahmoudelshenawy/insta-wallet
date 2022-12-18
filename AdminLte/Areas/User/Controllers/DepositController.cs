using AdminLte.Areas.Repositories;
using AdminLte.Areas.User.Models;
using AdminLte.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AdminLte.Areas.User.Controllers
{
    [Authorize(Policy = "User")]
    [Area("User")]
    [Route("/deposit")]
    public class DepositController : Controller
    {
        private readonly IDepositRepository _depositRepository;

        public DepositController(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository;
        }

        public async Task<IActionResult> Index()
        {
            var aCurrencies = await _depositRepository.ActiveCurrencyListForDeposit();
            var depositModel = new DepositViewModel
            {
                Currencies = aCurrencies.Select(c => new SelectListItem()
                {
                    Text = c.Code,
                    Value = c.Id.ToString()
                }).ToList()
            };

            return View(depositModel);
        }
        [HttpGet("getFeesLimitPaymentMethods/{currency_id}")]
        public async Task<IActionResult> getFeesLimitPaymentMethods(int currency_id)
        {
            var paymentMethods = await _depositRepository.ActivePaymentMethodsForSelectedCurrency(currency_id);
            var data = new
            {
                paymentMethods = paymentMethods
            };

            return Ok(data);
        }
        [AllowAnonymous]
        [HttpPost("getDepositAmountFeesLimit")]
        public async Task<IActionResult> getDepositAmountFeesLimit([FromBody] DepositViewModel viewModel)
        {

            var response = new Dictionary<string, decimal>();
            var error = new Dictionary<string, string>();

            if (ModelState.IsValid)
            {
                if (viewModel.PaymentMethodId != null && viewModel.CurrencyId != null)
                {
                    var feeLimit = await _depositRepository.getFeeLimitOfOneMethod(viewModel.CurrencyId, viewModel.PaymentMethodId);


                    response.Add("MinAmount", feeLimit.MinLimit);
                    response.Add("MaxAmount", feeLimit.MaxLimit ?? 0.00m);
                    response.Add("FixedFeeAmount", feeLimit.FixedAmount);
                    response.Add("PercentFeeAmount", feeLimit.PercentAmount);

                    if (viewModel.Amount > 0)
                    {
                        var amount = viewModel.Amount - feeLimit.FixedAmount +
                             (feeLimit.PercentAmount != null ? viewModel.Amount * feeLimit.PercentAmount / 100 : 0.00m);
                        var TotalFee = feeLimit.FixedAmount +
                             (feeLimit.PercentAmount != null ? viewModel.Amount * feeLimit.PercentAmount / 100 : 0.00m);

                        response.Add("TotalFees", TotalFee);
                        response.Add("TotalAmount", amount);

                        //  var balance = await _depositRepository.getUserBalance(User.FindFirstValue(ClaimTypes.NameIdentifier), viewModel.CurrencyId);

                        if (feeLimit.MinLimit > 0 && viewModel.Amount < feeLimit.MinLimit)
                        {
                            error.Add("status", "error");
                            error.Add("Amount", "less than min amount");
                        }
                        else if (feeLimit.MaxLimit != null && viewModel.Amount > feeLimit.MaxLimit)
                        {
                            error.Add("status", "error");
                            error.Add("Amount", "greater than max amount");
                        }

                    }

                }
                var data = new { response, error };
                return Ok(data);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Confirm(DepositViewModel depositView)
        {
            //Based on type get details
            //redirect to page
            var redirectTo = "";
            if (ModelState.IsValid == true)
            {
                var feeLimit = await _depositRepository.getFeeLimitOfOneMethod(depositView.CurrencyId, depositView.PaymentMethodId);
                var paymentMethod = await _depositRepository.getPaymentMethod(depositView.PaymentMethodId);
                redirectTo = paymentMethod.Name + "Confirm";

                switch (paymentMethod.Name)
                {
                    case "Bank":
                        //get bank list
                        var bankList = await _depositRepository.GetBankList(depositView.PaymentMethodId, depositView.CurrencyId);
                        depositView.Banks = bankList;
                        depositView.PaymentType = PaymentTypeEnum.Bank.ToString();
                        depositView.Status = PaymentStatus.Pending.ToString();
                        if (bankList.Count() == 0)
                        {
                            depositView.Error = true;
                            depositView.Message = "there are no bank exist in the selected currency";
                            return View("Index", depositView);
                        }
                        break;
                    default:
                        break;
                }

            }

            return View(redirectTo, depositView);
        }
        [HttpPost("bank/complete")]
        public async Task<IActionResult> CompleteBankDeposit(DepositViewModel depositView)
        {
            //validate model bankId and File upload
            //upload file 
            //prepare model for deposit
            //if success show success page
            //if fail show fail page
            if (depositView.File == null || depositView.PaymentTypeId == null || depositView.Amount <= 0)
            {
                ModelState.AddModelError("", "please fill all data required for deposit");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            depositView.UserId = userId;

            var result = await _depositRepository.CreateDepositSuccess(depositView);
            if (result == true)
            {

                return View("Success");
            }
            else
            {
                return View("Fail");
            }


        }

        [HttpGet("getBankDetail/{bank_id}")]
        public async Task<IActionResult> getBankDetail(int bank_id)
        {
            var bank = await _depositRepository.GetBankDetails(bank_id);
            return Ok(bank);
        }


    }
}
