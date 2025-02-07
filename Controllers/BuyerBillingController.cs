using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;
using System.Text.Json;

namespace Moksha_App.Controllers
{
    public class BuyerBillingController : Controller
    {

        private readonly HttpClient _httpClient;
        Uri baseAddress = new Uri("http://localhost:45753/api");

        public BuyerBillingController()
        {
            // Initialize HttpClient (ideally use IHttpClientFactory for better management)

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;


        }

        [HttpGet]
        public async Task<IActionResult> All_bills()
        {

            List<B_Bill> materials = new List<B_Bill>();
            string baseAdd = baseAddress + "/BuyerBilling";
            var token = Request.Cookies["AuthToken"];
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
    }
}
