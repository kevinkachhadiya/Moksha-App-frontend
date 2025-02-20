using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Moksha_App.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly HttpClient _client;
        private readonly IConfiguration _appsettings;

        public AuthController(ILogger<AuthController> logger, IConfiguration appsettings)
        {
            _logger = logger;
            _appsettings = appsettings;
            var backend_url = Environment.GetEnvironmentVariable("backend_url") ??_appsettings["BackendUrl"] ?? throw new ArgumentNullException("BackendUrl is not set.");
            Uri baseAddress = new Uri(backend_url);
            _client = new HttpClient { BaseAddress = baseAddress };
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string baseAdd = _client.BaseAddress + "/Auth/authenticate";
                var response = await _client.PostAsJsonAsync(baseAdd, model);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();

                    // Set a 24-hour expiration time
                    var cookieExpirationTime = DateTime.UtcNow.AddHours(24);

                    // Store the token securely in the cookie
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        HttpOnly = false,  // Prevent JavaScript access
                        Secure = true,  // Always use Secure in production
                        SameSite = SameSiteMode.None,// cross site
                        Expires = cookieExpirationTime, // expire after 24 hours
                    });

                    TempData["message"] = true;
                    return RedirectToAction("Dash_Board", "DashBoard");
                }
                else
                {
                    TempData["message"] = false;
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateToken()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No token found in cookies.");
            }

            // **FIX**: Do not assume token is JSON
            if (token.StartsWith("{"))
            {
                try
                {
                    token = System.Text.Json.JsonDocument.Parse(token)
                        .RootElement.GetProperty("token").GetString();
                }
                catch
                {
                    return Unauthorized("Invalid token format.");
                }
            }

            string baseAdd = _client.BaseAddress + "/Auth/ValidateToken";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, baseAdd);
            requestMessage.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _client.SendAsync(requestMessage);
            return response.IsSuccessStatusCode ? Ok("Token is valid") : Unauthorized("Token is invalid or expired");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
