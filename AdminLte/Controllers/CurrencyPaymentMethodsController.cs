using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Services;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
namespace AdminLte.Controllers
{
    [Route("admin/Currencies")]
    public class CurrencyPaymentMethodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IUploadService _UploadService;
        private readonly IWebHostEnvironment _appEnvironment;


        public CurrencyPaymentMethodsController(ApplicationDbContext context, IUploadService uploadService, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _UploadService = uploadService;
            _appEnvironment = appEnvironment;
        }

        public async Task<object> GetBasicData(int id, string pm)
        {
            pm = !string.IsNullOrEmpty(pm) ? pm : "Stripe";
            var currentCurrency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            var paymentMethod = await _context.PaymentMethods.FirstOrDefaultAsync(p => p.Name == pm);

            if (paymentMethod == null)
            {
                throw new PaymentMethodNotFoundException("payment method does not exist");
            }
            var currencies = await _context.Currencies.Select(x =>
          new
          {
              Id = x.Id,
              Name = x.Name
          }).ToListAsync();
            var data = new
            {
                Currencies = currencies,
                currentCurrency = currentCurrency,
                cmp = GetSelectedCurrencyPaymentMethods(id, paymentMethod).Result,
                pm = pm,
                PaymentMethodId = paymentMethod.Id
            };
            return data;
        }
        [HttpGet("payment-methods/{id}/{pm?}")]
        public async Task<IActionResult> Index(int id, string pm)
        {
            try
            {
                ViewBag.subItem = "Currencies";
                var data = await GetBasicData(id, pm);

                return View(data);
            }
            catch (PaymentMethodNotFoundException e)
            {
                var msg = e.Message;
                throw;
            }
        }
        [HttpPost("AddStripPaymentMethod")]
        public async Task<IActionResult> AddStripPaymentMethod(StripFormViewModel stripForm)
        {
            ViewBag.subItem = "Currencies";

            if (ModelState.IsValid)
            {
                var credentials = new
                {
                    SecretKey = stripForm.SecretKey,
                    PublishableKey = stripForm.PublishableKey
                };
                string credentialsJson = JsonSerializer.Serialize(credentials);
                string activatedFor;
                if (stripForm.Status == "Active")
                {
                    activatedFor = JsonSerializer.Serialize(stripForm.ActivatedFor);
                }
                else
                {
                    activatedFor = "[]";
                }

                //add or update
                var cpm = await _context.CurrencyPaymentMethods.
                    FirstOrDefaultAsync(cp => cp.CurrencyId == stripForm.CurrencyId && cp.PaymentMethodId == stripForm.PaymentMethodId);
                if (cpm != null)
                {
                    cpm.ActivatedFor = activatedFor;
                    cpm.MethodData = credentialsJson;
                    cpm.ProcessingTime = stripForm.ProcessingTime;
                    _context.CurrencyPaymentMethods.Update(cpm);
                }
                else
                {
                    var currencyPaymentMethod = new CurrencyPaymentMethod
                    {
                        CurrencyId = stripForm.CurrencyId,
                        PaymentMethodId = stripForm.PaymentMethodId,
                        ActivatedFor = activatedFor,
                        MethodData = credentialsJson,
                        ProcessingTime = stripForm.ProcessingTime
                    };
                    _context.CurrencyPaymentMethods.Add(currencyPaymentMethod);
                }
                await _context.SaveChangesAsync();

                ViewBag.IsSuccess = true;
            }
            var data = await GetBasicData(stripForm.CurrencyId, stripForm.Pm);
            return View("Index", data);
        }
        [HttpPost("AddPaypalPaymentMethod")]
        public async Task<IActionResult> AddPaypalPaymentMethod(PaypalFormViewModel paypalForm)
        {
            ViewBag.subItem = "Currencies";

            if (ModelState.IsValid)
            {
                var credentials = new
                {
                    ClientId = paypalForm.ClientId,
                    ClientSecret = paypalForm.ClientSecret,
                    Mode = paypalForm.Mode
                };
                string credentialsJson = JsonSerializer.Serialize(credentials);
                string activatedFor;
                if (paypalForm.Status == "Active")
                {
                    activatedFor = JsonSerializer.Serialize(paypalForm.ActivatedFor);
                }
                else
                {
                    activatedFor = "[]";
                }

                //add or update
                var cpm = await _context.CurrencyPaymentMethods.
                    FirstOrDefaultAsync(cp => cp.CurrencyId == paypalForm.CurrencyId && cp.PaymentMethodId == paypalForm.PaymentMethodId);
                if (cpm != null)
                {
                    cpm.ActivatedFor = activatedFor;
                    cpm.MethodData = credentialsJson;
                    cpm.ProcessingTime = paypalForm.ProcessingTime;
                    _context.CurrencyPaymentMethods.Update(cpm);
                }
                else
                {
                    var currencyPaymentMethod = new CurrencyPaymentMethod
                    {
                        CurrencyId = paypalForm.CurrencyId,
                        PaymentMethodId = paypalForm.PaymentMethodId,
                        ActivatedFor = activatedFor,
                        MethodData = credentialsJson,
                        ProcessingTime = paypalForm.ProcessingTime
                    };
                    _context.CurrencyPaymentMethods.Add(currencyPaymentMethod);
                }
                await _context.SaveChangesAsync();

                ViewBag.IsSuccess = true;
            }
            var data = await GetBasicData(paypalForm.CurrencyId, paypalForm.Pm);
            return View("Index", data);
        }
        [HttpPost("AddPaymobPaymentMethod")]
        public async Task<IActionResult> AddPaymobPaymentMethod(PaymobFormViewModel paymobForm)
        {
            ViewBag.subItem = "Currencies";

            if (ModelState.IsValid)
            {
                var credentials = new
                {
                    ApiKey = paymobForm.ApiKey,
                    IntegrationId = paymobForm.IntegrationId,
                    IframeId = paymobForm.IFrameId,
                    Hmac = paymobForm.Hmac,
                };
                string credentialsJson = JsonSerializer.Serialize(credentials);
                string activatedFor;
                if (paymobForm.Status == "Active")
                {
                    var activeFor = new[] { "Deposit" }; 
                    activatedFor = JsonSerializer.Serialize(activeFor);
                }
                else
                {
                    activatedFor = "";
                }

                //add or update
                var cpm = await _context.CurrencyPaymentMethods.
                    FirstOrDefaultAsync(cp => cp.CurrencyId == paymobForm.CurrencyId && cp.PaymentMethodId == paymobForm.PaymentMethodId);
                if (cpm != null)
                {
                    cpm.ActivatedFor = activatedFor;
                    cpm.MethodData = credentialsJson;
                    _context.CurrencyPaymentMethods.Update(cpm);
                }
                else
                {
                    var currencyPaymentMethod = new CurrencyPaymentMethod
                    {
                        CurrencyId = paymobForm.CurrencyId,
                        PaymentMethodId = paymobForm.PaymentMethodId,
                        ActivatedFor = activatedFor,
                        MethodData = credentialsJson,
                    };
                    _context.CurrencyPaymentMethods.Add(currencyPaymentMethod);
                }
                await _context.SaveChangesAsync();

                ViewBag.IsSuccess = true;
            }
            var data = await GetBasicData(paymobForm.CurrencyId, paymobForm.Pm);
            return View("Index", data);
        }
        [HttpPost("AddNewBank")]
        public async Task<IActionResult> AddNewBank(BankFormViewModel bankForm)
        {
            ViewBag.subItem = "Currencies";
            if (ModelState.IsValid)
            {
                var bank = new Bank
                {
                    CurrencyId = bankForm.CurrencyId,
                    BankBranchAddress = bankForm.BankBranchAddress,
                    AccountName = bankForm.AccountName,
                    AccountNumber = bankForm.AccountNumber,
                    Active = bankForm.Active,
                    BankBranchCity = bankForm.BankBranchCity,
                    BankBranchName = bankForm.BankBranchName,
                    CountryId = bankForm.CountryId,
                    IsDefault = bankForm.IsDefault,
                    SwiftCode = bankForm.SwiftCode,
                    BankName = bankForm.BankName,
                };

                if (bankForm.Logo != null)
                {
                    var file = new Attachment
                    {
                        Path = await _UploadService.UploadImage("uploads/files/banks/", bankForm.Logo),
                        Type = bankForm.Logo.ContentType
                    };

                    _context.Add(file);

                    //bank.AttachmentId = file.Id;
                    bank.Attachment = file;
                }
                _context.Banks.Add(bank);
                await _context.SaveChangesAsync();

                object method_data;
                string activated_for;
                if (bankForm.Active == true)
                {
                    method_data = new
                    {
                        bankId = bank.Id
                    };
                    var mtds = new string[] { "Deposit" };
                    activated_for = JsonSerializer.Serialize(mtds);
                }
                else
                {
                    method_data = new { };
                    var mtds = new string[] { };
                    activated_for = JsonSerializer.Serialize(mtds);
                }
                var newCurrencyPaymentMtd = new CurrencyPaymentMethod
                {
                    CurrencyId = bankForm.CurrencyId,
                    PaymentMethodId = bankForm.PaymentMethodId,
                    MethodData = JsonSerializer.Serialize(method_data),
                    ActivatedFor = activated_for
                };
                _context.Add(newCurrencyPaymentMtd);

                await _context.SaveChangesAsync();

            }
            var data = await GetBasicData(bankForm.CurrencyId, bankForm.Pm);
            ViewBag.IsSuccess = true;
            return View("Index", data);
        }
        [HttpGet("bank/{id}")]
        public async Task<IActionResult> getBankDetails(int id)
        {
            var bank = await _context.Banks.Include(b => b.Attachment).Where(b => b.Id == id).FirstOrDefaultAsync();
            var data = new
            {
                success = true,
                bank = bank
            };
            return new JsonResult(data);
        }
        [HttpPost("UpdateBank")]
        public async Task<IActionResult> UpdateBank(BankFormViewModel bankForm)
        {
            var bank = await _context.Banks.Include(b => b.Attachment).Where(b => b.Id == bankForm.BankId).FirstOrDefaultAsync();
            bank.BankName = bankForm.BankName;
            bank.AccountName = bankForm.AccountName;
            bank.AccountNumber = bankForm.AccountNumber;
            bank.BankBranchName = bankForm.BankBranchName;
            bank.BankBranchCity = bankForm.BankBranchCity;
            bank.BankBranchAddress = bankForm.BankBranchAddress;
            bank.SwiftCode = bankForm.SwiftCode;
            bank.Active = bankForm.Active;
            bank.IsDefault = bankForm.IsDefault;
            bank.CountryId = bankForm.CountryId;

            if (bankForm.Logo != null)
            {
                var file = new Attachment
                {
                    Path = await _UploadService.UploadImage("uploads/files/banks/", bankForm.Logo),
                    Type = bankForm.Logo.ContentType
                };

                _context.Add(file);
                if (bank.Attachment.Id != null)
                {
                    try
                    {
                        string webRootPath = _appEnvironment.WebRootPath;
                        //delete prev file
                        System.IO.File.Delete(Path.Combine(webRootPath, bank.Attachment.Path));
                    }
                    catch
                    {

                    }

                }

                bank.Attachment = file;
            }
            if (bankForm.Active == false)
            {
                //disable from deposit and update currencypaymentmethod
            }
            _context.Update(bank);
            await _context.SaveChangesAsync();
            var data = await GetBasicData(bankForm.CurrencyId, bankForm.Pm);
            ViewBag.IsUpdated = true;
            return View("Index", data);
        }
        [HttpDelete("DeleteBank/{id}/{methodId}")]
        public async Task<IActionResult> DeleteBank(int id, int methodId)
        {
            //delete attachment
            //delete currency payment method
            //delete bank
            var bank = await _context.Banks.Where(b => b.Id == id).FirstOrDefaultAsync();
            var file = await _context.Attachments.Where(a => a.Id == bank.Attachment.Id).FirstOrDefaultAsync();
            var currencyPaymentMethod = await _context.CurrencyPaymentMethods
                .Where(c => c.PaymentMethodId == methodId && c.CurrencyId == bank.CurrencyId)
                .ToListAsync();


            foreach (var paymentmtd in currencyPaymentMethod)
            {
                int BankId = (int)JObject.Parse(paymentmtd.MethodData)["bankId"];
                if(BankId == id)
                {
                    _context.Remove(paymentmtd);
                }
            }

            if (file != null)
                _context.Attachments.Remove(file);

            if (bank != null)
                _context.Banks.Remove(bank);

            await _context.SaveChangesAsync();

            var data = new
            {
                success = true
            };

            return new JsonResult(data);
        }
        private async Task<CurrencyPaymentMethodViewModel> GetSelectedCurrencyPaymentMethods(int currencyId, PaymentMethod paymentMethod)
        {
            var currencyPaymentViewModel = new CurrencyPaymentMethodViewModel()
            {
                Pm = paymentMethod.Name,
                CurrencyId = currencyId
            };

            var currencyPaymentMethod = await _context.CurrencyPaymentMethods
                .Where(cp => cp.PaymentMethodId == paymentMethod.Id && cp.CurrencyId == currencyId).ToListAsync();

            switch (paymentMethod.Name)
            {
                case "Stripe":
                    var stripFormModel = new StripFormViewModel()
                    {
                        Pm = paymentMethod.Name,
                        CurrencyId = currencyId,
                        PaymentMethodId = paymentMethod.Id
                    };
                    if (currencyPaymentMethod.Count > 0)
                    {
                        CurrencyPaymentMethod stripCurrencyPaymentMethod = currencyPaymentMethod[0];

                        if (stripCurrencyPaymentMethod != null)
                        {
                            stripFormModel.SecretKey = (string)JObject.Parse(stripCurrencyPaymentMethod.MethodData)["SecretKey"] ?? "";
                            stripFormModel.PublishableKey = (string)JObject.Parse(stripCurrencyPaymentMethod.MethodData)["PublishableKey"] ?? "";
                            stripFormModel.ProcessingTime = stripCurrencyPaymentMethod.ProcessingTime;
                            stripFormModel.ActivatedFor = JsonSerializer.Deserialize<string[]>(stripCurrencyPaymentMethod.ActivatedFor);
                            stripFormModel.Status = JsonSerializer.Deserialize<string[]>(stripCurrencyPaymentMethod.ActivatedFor).Count() > 0 ? "Active" : "Inactive";
                        }
                    }
                    currencyPaymentViewModel.StripFormViewModel = stripFormModel;
                    break;
                case "Paypal":
                    var paypalFormModel = new PaypalFormViewModel()
                    {
                        Pm = paymentMethod.Name,
                        CurrencyId = currencyId,
                        PaymentMethodId = paymentMethod.Id
                    };
                    if (currencyPaymentMethod.Count > 0)
                    {
                        CurrencyPaymentMethod paypalCurrencyPaymentMethod = currencyPaymentMethod[0];

                        if (paypalCurrencyPaymentMethod != null)
                        {
                            paypalFormModel.ClientId = (string)JObject.Parse(paypalCurrencyPaymentMethod.MethodData)["ClientId"] ?? "";
                            paypalFormModel.ClientSecret = (string)JObject.Parse(paypalCurrencyPaymentMethod.MethodData)["ClientSecret"] ?? "";
                            paypalFormModel.Mode = (string)JObject.Parse(paypalCurrencyPaymentMethod.MethodData)["Mode"] ?? "";
                            paypalFormModel.ProcessingTime = paypalCurrencyPaymentMethod.ProcessingTime;
                            paypalFormModel.ActivatedFor = JsonSerializer.Deserialize<string[]>(paypalCurrencyPaymentMethod.ActivatedFor);
                            paypalFormModel.Status = JsonSerializer.Deserialize<string[]>(paypalCurrencyPaymentMethod.ActivatedFor).Count() > 0 ? "Active" : "Inactive";
                        }
                    }
                    currencyPaymentViewModel.PaypalFormViewModel = paypalFormModel;
                    break;
                case "Paymob":
                    var paymobFormModel = new PaymobFormViewModel()
                    {
                        Pm = paymentMethod.Name,
                        CurrencyId = currencyId,
                        PaymentMethodId = paymentMethod.Id
                    };
                    if (currencyPaymentMethod.Count > 0)
                    {
                        CurrencyPaymentMethod paymobCurrencyPaymentMethod = currencyPaymentMethod[0];

                        if (paymobCurrencyPaymentMethod != null)
                        {
                            paymobFormModel.ApiKey = (string)JObject.Parse(paymobCurrencyPaymentMethod.MethodData)["ApiKey"] ?? "";
                            paymobFormModel.IntegrationId = (string)JObject.Parse(paymobCurrencyPaymentMethod.MethodData)["IntegrationId"] ?? "";
                            paymobFormModel.Hmac = (string)JObject.Parse(paymobCurrencyPaymentMethod.MethodData)["Hmac"] ?? "";
                            paymobFormModel.IFrameId = (string)JObject.Parse(paymobCurrencyPaymentMethod.MethodData)["IFrameId"] ?? "";
                            paymobFormModel.ActivatedFor = JsonSerializer.Deserialize<string[]>(paymobCurrencyPaymentMethod.ActivatedFor);
                           // paymobFormModel.Status = JsonSerializer.Deserialize<string>(paymobCurrencyPaymentMethod.ActivatedFor) != "" ? "Active" : "Inactive";
                            paymobFormModel.Status = JsonSerializer.Deserialize<string[]>(paymobCurrencyPaymentMethod.ActivatedFor).Count() > 0 ? "Active" : "Inactive";
                        }
                    }
                    currencyPaymentViewModel.PaymobFormViewModel = paymobFormModel;
                    break;
                case "Bank":
                    //check if this bank included in the currencyPaymentMethod

                    var ids = new List<int>();
                    foreach (var paymentmtd in currencyPaymentMethod)
                    {
                        int BankId = (int)JObject.Parse(paymentmtd.MethodData)["bankId"];
                        ids.Add(BankId);
                    }
                    var banks = _context.Banks.Where(b => b.CurrencyId == currencyId)
                        .Where(b => ids.Contains(b.Id))
                        .Select(bank => new BankFormViewModel()
                        {
                            Pm = paymentMethod.Name,
                            BankId = bank.Id,
                            CurrencyId = bank.CurrencyId,
                            PaymentMethodId = paymentMethod.Id,
                            AccountName = bank.AccountName,
                            AccountNumber = bank.AccountNumber,
                            BankBranchAddress = bank.BankBranchAddress,
                            BankBranchCity = bank.BankBranchCity,
                            BankBranchName = bank.BankBranchName,
                            BankName = bank.BankName,
                            CountryId = bank.CountryId,
                            SwiftCode = bank.SwiftCode,
                            IsDefault = bank.IsDefault,
                            ImagePath = bank.Attachment.Path
                        }).ToList();

                    currencyPaymentViewModel.Countries = await _context.Countries.
                        Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() })
                        .ToListAsync();
                    currencyPaymentViewModel.BankFormViewModels = banks;
                    currencyPaymentViewModel.PaymentMethodId = paymentMethod.Id;
                    break;
                default:
                    break;
            }
            return currencyPaymentViewModel;
        }
    }

    [Serializable]
    public class PaymentMethodNotFoundException : Exception
    {
        public PaymentMethodNotFoundException(string message) : base(message) { }
    }
}


//                       stripFormModel = new StripFormViewModel
//                        {
//                            SecretKey = (string)JObject.Parse(stripCurrencyPaymentMethod.MethodData)["secret_key"] ?? "",
//                            PublishableKey = (string)JObject.Parse(stripCurrencyPaymentMethod.MethodData)["publishable_key"] ?? "",
//                            ProcessingTime = stripCurrencyPaymentMethod.ProcessingTime,
//                            Status = (string)JObject.Parse(stripCurrencyPaymentMethod.ActivatedFor)["deposit"],

//,
//                        };