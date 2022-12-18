using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Services;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Controllers
{
    [Route("admin/[controller]")]
    public class CurrenciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUploadService _UploadService;

        public CurrenciesController(ApplicationDbContext context, IUploadService uploadService)
        {
            _context = context;
            _UploadService = uploadService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.subItem = "Currencies";

            var currencies =await _context.Currencies.ToListAsync();

            return View(currencies);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewBag.subItem = "Currencies";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Store(CurrencyViewModel viewModel)
        {
            ViewBag.subItem = "Currencies";
            if (ModelState.IsValid)
            {
                //do validation 
                var NameExists = _context.Currencies.FirstOrDefault(c => c.Name == viewModel.Name);
                var codeExists = _context.Currencies.FirstOrDefault(c => c.Code == viewModel.Code);
                var symbolExists = _context.Currencies.FirstOrDefault(c => c.Symbol == viewModel.Symbol);
                if (NameExists != null)
                {
                    ModelState.AddModelError("Name", "Name is already taken");
                }
                else if (codeExists != null)
                {
                    ModelState.AddModelError("Code", "Code is already taken");
                }
                else if (symbolExists != null)
                {
                    ModelState.AddModelError("Symbol", "Symbol is already taken");
                }
                else
                {
                    //if logo exists
                    if (viewModel.LogoFile != null)
                    {
                        viewModel.Logo = await _UploadService.UploadImage("uploads/currencies/", viewModel.LogoFile);
                    }

                    var newCurrency = new Currency
                    {
                        Type = "fiat",
                        Name = viewModel.Name,
                        Code = viewModel.Code,
                        Symbol = viewModel.Symbol,
                        Logo = viewModel.Logo,
                        Default = viewModel.Default,
                        Status = viewModel.Status,
                        ExchangeFrom = viewModel.ExchangeFrom
                    };

                    _context.Currencies.Add(newCurrency);
                    await _context.SaveChangesAsync();

                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                }

            }

            return View("Create");

        }
    }
}
