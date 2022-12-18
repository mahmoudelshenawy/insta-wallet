using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Controllers
{
    [Route("admin/[controller]")]
    public class PaymentMethodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentMethodsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            ViewBag.subItem = "PaymentMethods";
            string prop = "Name";
            string value = "Test";

            var paymentMethods = await _context.PaymentMethods
                .Select(p => new PaymentMethodViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Status = p.Status
                }).ToListAsync();
            return View(paymentMethods);
        }
        [HttpPost("store")]
        public async Task<IActionResult> Store(PaymentMethodViewModel viewModel)
        {
            ViewBag.subItem = "PaymentMethods";
            if (ModelState.IsValid)
            {
                //check if method exits
                var methodExists = await _context.PaymentMethods.Where(p => p.Name == viewModel.Name).FirstOrDefaultAsync();
                if(methodExists != null)
                {
                    ModelState.AddModelError("", "Payment Method already exists");
                }else
                {
                    var paymentMethod = new PaymentMethod
                    {
                        Name = viewModel.Name,
                        Status = viewModel.Status
                    };
                    _context.PaymentMethods.Add(paymentMethod);
                    _context.SaveChanges();
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                }
            }
           // return RedirectToAction("Index");
            var paymentMethods = await _context.PaymentMethods
               .Select(p => new PaymentMethodViewModel
               {
                   Id = p.Id,
                   Name = p.Name,
                   Status = p.Status
               }).ToListAsync();

            return View("Index", paymentMethods);
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update(PaymentMethodViewModel methodViewModel)
        {
            if(ModelState.IsValid)
            {
                var paymentMethod = await _context.PaymentMethods.Where(p => p.Id == methodViewModel.Id).FirstOrDefaultAsync();

                var methodExist = await _context.PaymentMethods.Where(p => p.Name == methodViewModel.Name.ToLower()).FirstOrDefaultAsync();
                if(methodExist != null)
                {
                    ModelState.AddModelError("Name", "payment method is already exists");
                }else
                {
                    if (paymentMethod.Name != methodViewModel.Name)
                    {
                        //TryUpdateModelAsync
                        paymentMethod.Name = methodViewModel.Name;
                        paymentMethod.Status = methodViewModel.Status;
                        _context.PaymentMethods.Update(paymentMethod);
                       await _context.SaveChangesAsync(true);

                        ViewBag.IsUpdated = true;
                    }

                }
              
            }
            var paymentMethods = await _context.PaymentMethods
              .Select(p => new PaymentMethodViewModel
              {
                  Id = p.Id,
                  Name = p.Name,
                  Status = p.Status
              }).ToListAsync();

            return View("Index", paymentMethods);
        }
        [HttpDelete("paymentMethods")]
        public async Task<IActionResult> Delete(int id)
        {
            var paymentMethod = await _context.PaymentMethods.Where(p=> p.Id == id).FirstOrDefaultAsync();
            if (paymentMethod == null)
                return NotFound();

          
                _context.PaymentMethods.Remove(paymentMethod);
               await _context.SaveChangesAsync(true);
          
            
            return Ok();
            //return RedirectToAction("Index" , new {IsDeleted = true});
        }
    }
}
