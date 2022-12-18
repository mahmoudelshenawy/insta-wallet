using AdminLte.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public PaymentMethodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var paymentMethod = await _context.PaymentMethods.Where(p => p.Id == id).FirstOrDefaultAsync();
            //if (paymentmethod == null)
            //    return notfound();


            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync(true);


            return Ok();
        }
    }
}
