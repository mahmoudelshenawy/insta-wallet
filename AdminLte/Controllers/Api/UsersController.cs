using AdminLte.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminLte.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _UserManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {

            _UserManager = userManager;

        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _UserManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var result = await _UserManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception();
            }
            return Ok();

        }
    }
}
