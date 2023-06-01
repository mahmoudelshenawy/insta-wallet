using AdminLte.Areas.Repositories;
using AdminLte.Areas.User.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Linq;
using System.Security.Claims;
using AdminLte.Controllers;
using Microsoft.Extensions.Localization;

namespace AdminLte.Areas.User.Controllers
{
    [Authorize(Policy = "User")]
    [Area("User")]
    [Route("/withdraw-money")]
    public class WithdrawMoneyController : Controller
    {
        private readonly IWithdrawalRepository _withdrawalRepo;
        const string SessionWithdrawModelKey = "SessionWithdrawModelKey";
        const string SessionFeesModelKey = "SessionFeesModelKey";
        private readonly IStringLocalizer<WithdrawMoneyController> __;
        public WithdrawMoneyController(IWithdrawalRepository withdrawalRepo, IStringLocalizer<WithdrawMoneyController> localizer)
        {
            _withdrawalRepo = withdrawalRepo;
            __ = localizer;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var data = await _withdrawalRepo.GetWithdrawalDetails();
            //return Ok(data);
            var paymentMethods = await _withdrawalRepo.GetPaymentMethods();
            var currencies = await _withdrawalRepo.GetCurrencies();
            var withdrawModel = new WithdrawViewModel();
            ViewBag.Countries = _withdrawalRepo.GetCountriesList().Result.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            withdrawModel.PaymentMethods = paymentMethods.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .Prepend(new SelectListItem { Text = "Please select a method", Value = "null" });

            ViewBag.SuccessMessage = __[(string)TempData["SuccessMessage"] ?? string.Empty];
            ViewBag.ErrorMessage = __[(string)TempData["ErrorMessage"] ?? string.Empty];
            return View(withdrawModel);
        }
        [HttpGet("get-active-currencies-for-method/{methodId}")]
        public async Task<IActionResult> GetActiveCurrenciesForSelectedPaymentMethod(int methodId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var currencies = await _withdrawalRepo.GetActiveCurrenciesForSelectedPaymentMethod(methodId, userId);
            var currencies = await _withdrawalRepo.GetCurrenciesList(methodId, userId);
            var data = new
            {
                success = true,
                currencies
            };
            return Ok(data);
        }

        [HttpPost("CheckAmountFeesAndBalance")]
        public async Task<IActionResult> CheckAmountFeesAndBalance([FromBody] JsonElement json)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var body = json.ToString();
            var response = await _withdrawalRepo.GetFeesLimitAndCheckBalanceAvailability(body, _userId);


            //option B ==> Body Stream
            //object data;
            //using (var reader = new StreamReader(Request.Body))
            //{
            //    string body = await reader.ReadToEndAsync();
            //    var amount = (int)JObject.Parse(body)["amount"];
            //    var currencyId = (int)JObject.Parse(body)["currencyId"];
            //    var paymentMethodId = (int)JObject.Parse(body)["paymentMethodId"];

            //     data = new
            //    {
            //        Success = true,
            //        amount,
            //        currencyId,
            //        paymentMethodId
            //    };
            //}

            return Ok(response);
        }
        [HttpGet("get-payout-settings/{paymentMethodId}")]
        public async Task<IActionResult> GetUserPayoutSettings(int paymentMethodId)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payoutSettings = await _withdrawalRepo.GetPayoutSettings(_userId , paymentMethodId);
            return Ok(payoutSettings);
        }
        [HttpPost("submit-payout")]
        public async Task<IActionResult> SubmitPayoutSettingForm(WithdrawViewModel model)
        {
            //check again about feesLimit and balance
            //if true go for confirmation
            //if false redirect back 
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _withdrawalRepo.CheckBalanceAvailability(model, _userId);
            if (response.Success)
            {
                ViewBag.SuccessMessage = "Please Confirm the Process";
                TempData["SuccessMessage"] = "Please Confirm the Process";

                //edit withdraw model to add extra edit
                model.Currency = response.DataSet.FirstOrDefault(x => x.Key == "Currency").Value;
                model.PaymentMethod = response.DataSet.FirstOrDefault(x => x.Key == "PaymentMethod").Value;

                model.FixedFeeAmount = response.DataSetNumbers.FirstOrDefault(x => x.Key == "FixedFeeAmount").Value;
                model.PercentFeeAmount = response.DataSetNumbers.FirstOrDefault(x => x.Key == "PercentFeeAmount").Value;
                model.TotalFees = response.DataSetNumbers.FirstOrDefault(x => x.Key == "TotalFees").Value;
                model.TotalAmount = response.DataSetNumbers.FirstOrDefault(x => x.Key == "TotalAmount").Value;

                HttpContext.Session.SetString("SessionWithdrawModelKey", JsonSerializer.Serialize(model));

                return RedirectToAction("Confirm");
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            return Ok(response);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm()
        {
            try
            {
                var sessionWithdrawModel = HttpContext.Session.GetString(SessionWithdrawModelKey);
                var withdrawViewModel = JsonSerializer.Deserialize<WithdrawViewModel>(sessionWithdrawModel);

                if (sessionWithdrawModel == null)
                {
                    TempData["ErrorMessage"] = "Failed Process";
                    return RedirectToAction("Index");
                }

                ViewBag.SuccessMessage = __[(string)TempData["SuccessMessage"] ?? string.Empty];
                return View(withdrawViewModel);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Failed Process";
                HttpContext.Session.Remove(SessionWithdrawModelKey);
                return RedirectToAction("Index");

            }

        }

        [HttpPost("success")]
        public async Task<IActionResult> WithdrawSuccess()
        {
            try
            {
                var sessionWithdrawModel = HttpContext.Session.GetString(SessionWithdrawModelKey);
                var withdrawViewModel = JsonSerializer.Deserialize<WithdrawViewModel>(sessionWithdrawModel);
                var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (withdrawViewModel == null)
                {
                    TempData["ErrorMessage"] = "Failed Process";
                    HttpContext.Session.Remove(SessionWithdrawModelKey);
                    return RedirectToAction("Index");
                }
                var response = await _withdrawalRepo.SubmitWithdrawalAndCreateTransaction(withdrawViewModel, _userId);

                if (!response) {
                    TempData["ErrorMessage"] = "Failed Process";
                    HttpContext.Session.Remove(SessionWithdrawModelKey);
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Success");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed Process";
                HttpContext.Session.Remove(SessionWithdrawModelKey);
                return RedirectToAction("Index");
            }

        }

        [HttpGet("success")]
        public async Task<IActionResult> Success()
        {
            var sessionWithdrawModel = HttpContext.Session.GetString(SessionWithdrawModelKey);
            var withdrawViewModel = JsonSerializer.Deserialize<WithdrawViewModel>(sessionWithdrawModel);
            return View(withdrawViewModel);
        }

        [HttpGet("failed")]
        public IActionResult FailedWithdrawal()
        {
            return View("Fail");
        }
    }
}
