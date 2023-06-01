using AdminLte.Areas.Repositories;
using AdminLte.Areas.User.Models;
using AdminLte.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Web;

namespace AdminLte.Areas.User.Controllers
{
    [Authorize(Policy = "User")]
    [Area("User")]
    [Route("/deposit")]
    public class DepositController : Controller
    {
        const string SessionKeyName = "TransactionInfo";
        private readonly IDepositRepository _depositRepository;
        private readonly IStripeRepository _stripeRepository;

        public DepositController(IDepositRepository depositRepository, IStripeRepository stripeRepository)
        {
            _depositRepository = depositRepository;
            _stripeRepository = stripeRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var test = await _depositRepository.TestFeature();

            return Ok(test);
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
                            depositView.PaymentMethodId = paymentMethod.Id;
                            depositView.Message = "there are no bank exist in the selected currency";
                            return View("Index", depositView);
                        }
                        break;
                    case "Stripe":
                        depositView.PaymentType = PaymentTypeEnum.Stripe.ToString();
                        depositView.Status = PaymentStatus.Success.ToString();
                        redirectTo = "Confirm";
                        break;
                    case "Paypal":
                        depositView.PaymentType = PaymentTypeEnum.Paypal.ToString();
                        depositView.Status = PaymentStatus.Success.ToString();
                        redirectTo = "Confirm";
                        break;

                    default:
                        break;
                }

            }

            return View(redirectTo, depositView);
        }
        [HttpPost("confirm")]
        public async Task<IActionResult> CompleteDeposit(DepositViewModel depositView)
        {
            switch (depositView.PaymentType)
            {
                case "Bank":
                    return await CompleteBankDeposit(depositView);
                    break;
                case "Stripe":
                    return await CompleteStripeDeposit(depositView);
                    break;
                case "Paypal":
                    return await CompletePaypalDeposit(depositView);
                    break;
                default:
                    break;
            }
            return Ok();
        }
        [HttpGet("success")]
        public IActionResult DepositSuccess(AmountModel success)
        {
            return View("Success", success);
        }
        [HttpGet("fail")]
        public IActionResult DepositFail(AmountModel success)
        {
            return View("Fail");
        }

        [HttpPost("visa/form")]
        public async Task<IActionResult> CompleteStripeDeposit(DepositViewModel depositView)
        {

            var email = await _depositRepository.getUserEmail(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var StripeViewForm = new StripeViewModel
            {
                PaymentMethodId = depositView.PaymentMethodId,
                Amount = depositView.Amount,
                FixedFeeAmount = depositView.FixedFeeAmount,
                CurrencyId = depositView.CurrencyId,
                PercentFeeAmount = depositView.PercentFeeAmount,
                TotalFees = depositView.TotalFees,
                Status = depositView.Status,
                Email = email,
            };

            HttpContext.Session.SetString(SessionKeyName, JsonSerializer.Serialize<StripeViewModel>(StripeViewForm));

            return View("StripeForm", StripeViewForm);
        }
        public async Task<IActionResult> CompletePaypalDeposit(DepositViewModel depositView)
        {
            //get the currency code
            //get the client id
            var currency = await _depositRepository.getCurrencyCode(depositView.CurrencyId);
            var clientId = await _depositRepository.GetPaypalClientId(depositView);
            var paypalViewForm = new PaypalViewModel
            {
                PaymentMethodId = depositView.PaymentMethodId,
                Amount = depositView.Amount,
                FixedFeeAmount = depositView.FixedFeeAmount,
                CurrencyId = depositView.CurrencyId,
                PercentFeeAmount = depositView.PercentFeeAmount,
                TotalFees = depositView.TotalFees,
                Status = depositView.Status,
                Currency = currency.ToUpper(),
                ClientId = clientId
            };
            return View("PaypalForm", paypalViewForm);
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
                var currency = await _depositRepository.getCurrencyCode(depositView.CurrencyId);

                return RedirectToAction("DepositSuccess",
                    new AmountModel { Amount = (depositView.Amount - depositView.TotalFees) + " " + currency });
                //return View("Success", new
                //{
                //    Amount = (depositView.Amount - depositView.TotalFees) + " " + currency,
                //});
            }
            else
            {
                return RedirectToAction("DepositFail");
                // return View("Fail");
            }


        }

        [HttpGet("getBankDetail/{bank_id}")]
        public async Task<IActionResult> getBankDetail(int bank_id)
        {
            var bank = await _depositRepository.GetBankDetails(bank_id);
            return Ok(bank);
        }
        [HttpPost("visa/success")]
        public async Task<IActionResult> MakeStripePayment(StripeViewModel stripeView)
        {
            var currency = await _depositRepository.getCurrencyCode(stripeView.CurrencyId);
            var feeLimit = await _depositRepository.getFeeLimitOfOneMethod(stripeView.CurrencyId, stripeView.PaymentMethodId);

            stripeView.FixedFeeAmount = feeLimit.FixedAmount;
            stripeView.PercentFeeAmount = feeLimit.PercentAmount;
            stripeView.Currency = currency;
            stripeView.TotalFees = feeLimit.FixedAmount +
                           (feeLimit.PercentAmount != null ? stripeView.Amount * feeLimit.PercentAmount / 100 : 0.00m);

            HttpContext.Session.SetString(SessionKeyName, JsonSerializer.Serialize<StripeViewModel>(stripeView));
            if (ModelState.IsValid)
            {
                //create payment method
                //create payment intent
                //confirm payment intent
                //watch result status

                var result = await _stripeRepository.CreatePaymentAction(stripeView);

                if (result.Error == true)
                {
                    return RedirectToAction(nameof(DepositFail));
                    //return View("Fail");
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.RedirectToStripe))
                    {
                        return Redirect(result.RedirectToStripe);
                    }
                    else
                    {
                        var depositViewModel = new DepositViewModel
                        {
                            Amount = stripeView.Amount,
                            CurrencyId = stripeView.CurrencyId,
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            EarlyPay = true,
                            PaymentMethodId = stripeView.PaymentMethodId,
                            TotalFees = stripeView.TotalFees,
                            Status = stripeView.Status,
                            FixedFeeAmount = stripeView.FixedFeeAmount,
                            PercentFeeAmount = stripeView.PercentFeeAmount,
                            PaymentType = PaymentTypeEnum.Stripe.ToString()
                        };
                        var paymentResult = await _depositRepository.CreateDepositSuccess(depositViewModel);
                        if (paymentResult)
                        {
                            return RedirectToAction("DepositSuccess",
                    new AmountModel { Amount = (stripeView.Amount - stripeView.TotalFees) + " " + stripeView.Currency });

                            //return View("Success", new
                            //{
                            //    Amount = (stripeView.Amount - stripeView.TotalFees) + stripeView.Currency
                            //});
                        }
                        else
                        {
                            return RedirectToAction(nameof(DepositFail));
                            //return View("Fail");
                        }

                    }
                }
            }
            return View("StripeForm", stripeView);
        }
        [HttpGet("visa/callback")]
        public async Task<IActionResult> StripeAuthenticationCallback()
        {
            var request = HttpContext.Request;
            string payment_intent = HttpUtility.ParseQueryString(request.QueryString.ToString()).Get("payment_intent");
            if (!string.IsNullOrEmpty(payment_intent))
            {
                var session = HttpContext.Session.GetString(SessionKeyName);
                var transInfo = JsonSerializer.Deserialize<StripeViewModel>(session);
                var status = _stripeRepository.CheckPaymentValidationAfterRedirectBack(payment_intent, transInfo);
                if (status == "succeeded")
                {
                    var depositViewModel = new DepositViewModel
                    {
                        Amount = transInfo.Amount,
                        CurrencyId = transInfo.CurrencyId,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        EarlyPay = true,
                        PaymentMethodId = transInfo.PaymentMethodId,
                        TotalFees = transInfo.TotalFees,
                        Status = transInfo.Status,
                        FixedFeeAmount = transInfo.FixedFeeAmount,
                        PercentFeeAmount = transInfo.PercentFeeAmount,
                        PaymentType = PaymentTypeEnum.Stripe.ToString()
                    };
                    var paymentResult = await _depositRepository.CreateDepositSuccess(depositViewModel);
                    if (paymentResult)
                    {
                        return RedirectToAction("DepositSuccess",
                   new AmountModel { Amount = (transInfo.Amount - transInfo.TotalFees) + transInfo.Currency });
                        //return View("Success", new
                        //{
                        //    Amount = (transInfo.Amount - transInfo.TotalFees) + transInfo.Currency
                        //});
                    }
                    else
                    {
                        return RedirectToAction(nameof(DepositFail));
                    }
                }
                else if (status == "requires_payment_method")
                {
                    return RedirectToAction(nameof(DepositFail));
                }
            }
            return RedirectToAction("Index");

            // return View();
        }

        [HttpPost("paypal/success")]
        public async Task<IActionResult> PayalPaymentSuccess([FromBody] PaypalViewModel paypalView)
        {
            var depositViewModel = new DepositViewModel
            {
                Amount = paypalView.Amount,
                CurrencyId = paypalView.CurrencyId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                EarlyPay = true,
                PaymentMethodId = paypalView.PaymentMethodId,
                TotalFees = paypalView.TotalFees,
                Status = paypalView.Status,
                FixedFeeAmount = paypalView.FixedFeeAmount,
                PercentFeeAmount = paypalView.PercentFeeAmount,
                PaymentType = PaymentTypeEnum.Paypal.ToString()
            };
            var paymentResult = await _depositRepository.CreateDepositSuccess(depositViewModel);

            if (paymentResult)
            {
                var data = new { data = new AmountModel { Amount = paypalView.Amount + paypalView.Currency }, success = true };
                return Ok(data);
            }
            else
            {
                var data = new { success = false };
                return Ok(data);
            }

        }
    }
}
