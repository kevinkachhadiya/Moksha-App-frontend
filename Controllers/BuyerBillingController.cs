using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moksha_App.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Moksha_App.Controllers
{
    public class BuyerBillingController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _appsettings;
        public BuyerBillingController(IConfiguration appsettings)
        {
            _appsettings = appsettings;
            var backend_url = Environment.GetEnvironmentVariable("backend_url")
                 ?? _appsettings["BackendUrl"] ?? "";
            Uri baseAddress = new Uri(backend_url);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> All_bills()
        {

            List<B_Bill> materials = new List<B_Bill>();
            string baseAdd = _httpClient.BaseAddress + "/BuyerBilling";
            var token = Request.Cookies["AuthToken"] ?? "";
            token = System.Text.Json.JsonDocument.Parse(token).RootElement.GetProperty("token").GetString();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                // Call the API
                HttpResponseMessage response = await _httpClient.GetAsync(baseAdd); // Replace with your API route

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the JSON response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    materials = System.Text.Json.JsonSerializer.Deserialize<List<B_Bill>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    // Handle API errors
                    ViewBag.Error = "Failed to retrieve materials. Status Code: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.Error = "An error occurred while calling the API: " + ex.Message;
            }

            // Pass the data to the view
            return View(materials);
        }

    
        [HttpPost]
        public async Task<IActionResult> CreateBill([FromForm] Create_B_Bill_Dto bill)
        {
          
            string jsonContent = JsonSerializer.Serialize(bill);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                string baseAdd = _httpClient.BaseAddress + "/BuyerBilling";
                var response = await _httpClient.PostAsync(baseAdd, content);
               

                if (response.IsSuccessStatusCode)

                {
                //var token = await response.Content.ReadAsStringAsync();

                // Set a 24-hour expiration time
                // var cookieExpirationTime = DateTime.UtcNow.AddHours(24);

                return Ok(new
                {
                    success = true,
                    message = "Buying bill generated successfully"
                });
            }
                else
                {
                    
                    return Json(new { redirect = "Dash_Board" }); // Fix: Use a single object
                }
           
        }

        [HttpGet]
        public async Task<IActionResult> PrintBill(int B_id)
        {
            string baseAdd = _httpClient.BaseAddress + $"/BuyerBilling/print_buying_bile?billId={B_id}";
            var response = await _httpClient.GetAsync(baseAdd);
            if (response.IsSuccessStatusCode)

            {
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/pdf";
                var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('\"') ?? "invoice.pdf";

                return File(fileBytes, contentType, fileName);
            
            }
            else
            {
               
                return Json(new { redirect = "Dash_Board" }); // Fix: Use a single object
            }
        }

    }
}