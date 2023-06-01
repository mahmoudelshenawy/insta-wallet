using AdminLte.Models;
using AdminLte.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminLte.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthenticationController : Controller
    {
        private readonly IApiAuthenticationRepository _authRepo;

        public AuthenticationController(IApiAuthenticationRepository authRepo)
        {
            _authRepo = authRepo;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var data = new { Success = true };
            return Ok(data);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepo.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepo.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (result.RefreshToken != null)
            {
                SetRefreshTokenToCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var token = Request.Cookies["refreshToken"];

            var result = await _authRepo.RefreshTokenAsync(token);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenToCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken revokeToken)
        {
            var token = revokeToken.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }
            var result = await _authRepo.RevokeTokenAsync(token);
            if (!result)
                return BadRequest("Token is invalid");


            return Ok();
        }

        public void SetRefreshTokenToCookie(string refresh_token, DateTime expiresOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refresh_token, cookieOptions);
        }

    }
}
