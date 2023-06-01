using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
//using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace AdminLte.Areas.User.Controllers
{
    //[Authorize(Policy = "User")]
    [Area("User")]
    [Route("/http")]
    public class HttpClientController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public HttpClientController(IWebHostEnvironment env , ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var url = "http://localhost/Lirat/api/test";
            using (var httpClient = new HttpClient())
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var response = await httpClient.GetAsync(url);
                var data = await response.Content.ReadAsStringAsync();
                var dummy = JsonSerializer.Deserialize<Dummy>(data, options);
                return Ok(dummy);
            }
        }
        [HttpGet("filter")]
        public IActionResult Filter()
        {
            var records = new List<UserDto>
            {
                new UserDto { Id = 1 , CurrencyId =1},
                new UserDto { Id = 2 , CurrencyId =1},
                new UserDto { Id = 3 , CurrencyId =1},
                new UserDto { Id = 4 , CurrencyId =1},
                new UserDto { Id = 5 , CurrencyId =1},
                new UserDto { Id = 6 , CurrencyId =1},
            };
            var filtered = records.Filter((e) => e.Id == 1);

            var tableName = "Deposits";
       

                return Ok();
        }
        [HttpGet("wallet")]
        public async Task<IActionResult> Post()
        {
            var url = "http://localhost/Lirat/api/test/wallet";
            var user = new UserDto { CurrencyId = 1, Id = 1 };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(url, content);
                var responseText = await result.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = new FromSnakeCaseToPascalNamingPolicy(),
                };
                if (result.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    var errors = GetErrorMessagesFromResponse(responseText);
                    return Ok(errors);
                }

                responseText = new string((from c in responseText where c != '_' select c).ToArray());
                var wallet = JsonSerializer.Deserialize<WalletDto>(responseText, options);
                return Ok(wallet);
            }
        }
        [HttpGet("secure")]
        public async Task<IActionResult> GetSecureConnection()
        {
            var get_token = "http://localhost/Lirat/api/test/get-token";
            var secure = "http://localhost/Lirat/api/test/secure";

            var user = new UserDto { CurrencyId = 1, Id = 2 };
            using (var httpClient = new HttpClient())
            {
                var responseToken = await httpClient.PostAsJsonAsync(get_token, user);
                var responseText = await responseToken.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseText);
                var token = responseJson.GetProperty("token");
                using (var httpMessage = new HttpRequestMessage(HttpMethod.Get, secure))
                {
                    httpMessage.Headers.Add("Authorization-Token", token.ToString());
                    var secureJson = await httpClient.SendAsync(httpMessage);
                    var secureResponseText = await secureJson.Content.ReadAsStringAsync();
                    return Ok(secureResponseText);
                }
                return Ok(token);
            }

        }
        [HttpPost("upload")]
        public async Task<IActionResult> SaveFileFromApi([FromForm] IFormFile file)
        {
            var upload = "http://localhost/Lirat/api/test/upload";
            using var requestContent = new MultipartFormDataContent();

            using var fileRead = file.OpenReadStream();

            requestContent.Add(new StreamContent(fileRead), "file", file.FileName);


            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(upload, requestContent);
                var responseText = await response.Content.ReadAsStringAsync();
                return Ok(responseText);
            }
            //  var uploaded = await SaveFile(file, "ToTrash");

            return Ok();
        }
        private async Task<bool> SaveFile(IFormFile file, string folderName)
        {
            var filename = $"{Guid.NewGuid()}.{Path.GetExtension(file.FileName)}";
            var route = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }
            var filePath = Path.Combine(route, filename);
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(fileStream);
            }

            return true;
        }
        private static Dictionary<string, List<string>> GetErrorMessagesFromResponse(string body)
        {
            var response = new Dictionary<string, List<string>>();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(body);
            var errorsFromJson = jsonElement.GetProperty("errors");

            foreach (var fieldWithError in errorsFromJson.EnumerateObject())
            {
                var field = fieldWithError.Name;
                var errorList = new List<string>();
                foreach (var errorSingle in fieldWithError.Value.EnumerateArray())
                {
                    errorList.Add(errorSingle.ToString());
                }
                response.Add(field, errorList);
            }
            return response;
        }
    }

    public class Dummy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public int Balance { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
    }
    // [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class WalletDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CurrencyId { get; set; }
        public string Balance { get; set; }
        public string Invested { get; set; }
        public string IsDefault { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class FromSnakeCaseToPascalNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            StringBuilder resultBuilder = new StringBuilder();

            foreach (char c in name)
            {
                // Replace anything, but letters and digits, with space
                if (!Char.IsLetterOrDigit(c))
                {
                    resultBuilder.Append(" ");
                }
                else
                {
                    resultBuilder.Append(c);
                }
            }

            string result = resultBuilder.ToString();

            // Make result string all lowercase, because ToTitleCase does not change all uppercase correctly
            result = result.ToLower();

            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            result = myTI.ToTitleCase(result).Replace(" ", String.Empty);
            return result;
        }

    }

}
