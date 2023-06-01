using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AdminLte.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("hello world");
        }
        [HttpGet("data")]
        [AllowAnonymous]
        public IActionResult Data()
        {
            return Ok(new
            {
                success = "Hello World"
            });
        }
    }
}
