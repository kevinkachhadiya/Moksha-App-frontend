using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Moksha_App.Controllers
{
    public class Stock_Management_Controller : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _appsettings;
        Uri uri;
        public Stock_Management_Controller(IConfiguration ic)
        {
            _appsettings = ic;
            var backend_url = Environment.GetEnvironmentVariable("backend_url")
                 ?? _appsettings["BackendUrl"] ?? "";

            _httpClient = new HttpClient();
            uri = new Uri(backend_url);
        }

        [HttpGet]

        public async Task<IActionResult> GetAllStocks(string searchTerm = "",
             string sortColumn = "ColorName",
             string sortDirection = "asc",
             int page = 1,
             int pageSize = 12)
        {
            string baseAdd = uri + "/Stock_Management/GetAllStocks";
            var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryParams["searchTerm"] = searchTerm ?? "";
            queryParams["sortColumn"] = sortColumn ?? "ColorName";
            queryParams["sortDirection"] = sortDirection ?? "";
            queryParams["page"] = page.ToString() ?? "1";
            queryParams["pageSize"] = pageSize.ToString() ?? "12";

            string fullUrl = $"{baseAdd}?{queryParams}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var stocks = System.Text.Json.JsonSerializer.Deserialize<stockListViewModel>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Debug.WriteLine(JsonSerializer.Serialize(stocks));
                    return View(stocks);
                }

                else
                {
                    // Log actual response body for debugging
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Failed to retrieve materials. Status: {response.StatusCode}. Error: {errorResponse}";
                    return View();

                }


            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.Error = "An error occurred while calling the API: " + ex.Message;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Stock s)
        {

            string baseAdd = uri + "/Stock_Management/CreateStock";


            var content = new StringContent(
                                JsonSerializer.Serialize(s),
                                Encoding.UTF8,
                               "application/json"
                               );

            var response = await _httpClient.PostAsync(baseAdd, content);
            var errorDetails = await response.Content.ReadAsStringAsync();


            if (response.StatusCode == HttpStatusCode.Created) // Check for 201 Created
            {
                TempData["Success"] = "stock '" + "" + "' added!";
                return Json(new { success = true });
            }
            else
            {
                return StatusCode(500, new { success = false, message = errorDetails });
            }

        }

    }
}