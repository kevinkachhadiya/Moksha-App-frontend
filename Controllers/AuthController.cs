using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
            var backend_url = _appsettings["BackendUrl"]??"";
            Uri baseAddress = new Uri(backend_url);
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    // Store the token securely (e.g., in a cookie or session)
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        HttpOnly = true, // Prevent JavaScript access
                        Secure = false,  // Change to 'true' if using HTTPS
                        SameSite = SameSiteMode.Lax // Prevent cookie deletion on cross-site navigation
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

            // Decode the token if it is JSON encoded
            token = System.Text.Json.JsonDocument.Parse(token).RootElement.GetProperty("token").GetString();

            string baseAdd = _client.BaseAddress + "/Auth/ValidateToken";

            // Create a new HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, baseAdd)
            {
                // Set Authorization header with Bearer token

                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Send the request and get the response
            var response = await _client.SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok("Token is valid");

            }
            else
            {
                return Unauthorized("Token is invalid or expired");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
   
   }
}