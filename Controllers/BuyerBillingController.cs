using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moksha_App.Models;
using NuGet.Protocol;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Moksha_App.Models.Party;

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
        public async Task<IActionResult> All_bills(
             string searchTerm = "",
             string sortColumn = "CreatedAt",
             string sortDirection = "desc",
             int page = 1,
             int pageSize = 10)
        {
            string baseAdd = _httpClient.BaseAddress + "/BuyerBilling";

            var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryParams["searchTerm"] = searchTerm;
            queryParams["sortColumn"] = sortColumn;
            queryParams["sortDirection"] = sortDirection;
            queryParams["page"] = page.ToString();
            queryParams["pageSize"] = pageSize.ToString();

            string fullUrl = $"{baseAdd}?{queryParams}";

            HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);


            if (response.IsSuccessStatusCode)
            {

                string jsonResponse = await response.Content.ReadAsStringAsync();


                var viewModel = System.Text.Json.JsonSerializer.Deserialize<BillListViewModel>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(viewModel);
            }
            else
            {
                // Handle API errors
                ViewBag.Error = "Failed to retrieve materials. Status Code: " + response.StatusCode;
                return View();
            }





            /* List<B_Bill> materials = new List<B_Bill>();

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
             return View(materials);*/
        }


        [HttpPost]
        public async Task<IActionResult> CreateBill([FromForm] Create_B_Bill_Dto bill)
        {
            bill.P_number = bill.P_number ?? "0000000000";

            string jsonContent = JsonSerializer.Serialize(bill);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            string baseAdd = _httpClient.BaseAddress + "/BuyerBilling";
            var response = await _httpClient.PostAsync(baseAdd, content);

            var message = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)

            {
                return Ok(new
                {
                    success = true,
                    message = "Buying bill generated successfully"
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = message
                });
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

        [HttpGet("GetBillById")]
        public async Task<IActionResult> GetBillById(int B_id)
        {

            string baseAdd = _httpClient.BaseAddress + $"/BuyerBilling/GetBillByid?billId={B_id}";

            var response = await _httpClient.GetAsync(baseAdd);

            string jsonResponse = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<Create_B_Bill_Dto>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (response.IsSuccessStatusCode)
            {
                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            else
            {
                return Json(new { redirect = "Dash_Board" }); // Fix: Use a single object
            }
        }

        [HttpPut]
        public async Task<IActionResult> Updatedbuyingbill([FromForm] Edit_B_Bill_Dto bill)
        {
            string jsonContent = JsonSerializer.Serialize(bill);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            string baseAdd = _httpClient.BaseAddress + "/BuyerBilling/UpdateBill";
            var response = await _httpClient.PutAsync(baseAdd, content);

            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { success = true, message = "Bill updated successfully" });
            }
            return Ok(new { success = false, message = message });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBill(int B_id)
        {
            string baseAdd = _httpClient.BaseAddress + $"/BuyerBilling/Deletebill?billId={B_id}";
            var response = await _httpClient.DeleteAsync(baseAdd);

            return Ok(new { success = true, message = "Bill Deleted successfully" });
        }

    }
}