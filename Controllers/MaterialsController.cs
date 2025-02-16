using System;
using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;
using Newtonsoft.Json;

namespace Moksha_App.Controllers
{
    [Route("[controller]")]
    public class MaterialsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _appsettings;
        Uri uri;

        public MaterialsController(IConfiguration ic)
        {
            _appsettings = ic;
            var backend_url = Environment.GetEnvironmentVariable("backend_url")
                 ?? _appsettings["BackendUrl"] ?? "";

            _httpClient = new HttpClient();
             uri = new Uri(backend_url);
        }
        // GET: All Materials
        [HttpGet]
        public async Task<IActionResult> All_Materials()
        {
            List<Material> materials = new List<Material>();
            string baseAdd = uri + "/Materials";
             var token = Request.Cookies["AuthToken"] ?? "";
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    token = JsonDocument.Parse(token).RootElement.TryGetProperty("token", out JsonElement tokenElement)
                        ? tokenElement.GetString()
                        : throw new Exception("Invalid token structure.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token parsing error: {ex.Message}");
                }
            }

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
                    materials = System.Text.Json.JsonSerializer.Deserialize<List<Material>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });             }
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
        public async Task<IActionResult> Create(Material material)
        {
            string baseAdd = uri + "/Materials/CreateMaterial";
            var token = Request.Cookies["AuthToken"]?? "";
            token = System.Text.Json.JsonDocument.Parse(token).RootElement.GetProperty("token").GetString();

            // Set up headers for the request (e.g., application/json)
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // add token in the header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Serialize the material object into JSON
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(material), Encoding.UTF8, "application/json");


            // Send the POST request to the API
            var response = await _httpClient.PostAsync(baseAdd, content);
           

            if (response.StatusCode == HttpStatusCode.Created) // Check for 201 Created
            {
                TempData["Success"] = "Material '" + material.ColorName + "' added!";


                // If creation is successful, redirect to "All_Materials"
                return RedirectToAction("All_Materials");
            }
            else
            {
                ViewBag.Error = "Error occurred while creating the material.";
                return RedirectToAction("All_Materials"); // Default redirect in case of failure            
            }

            

        }
        [HttpDelete]
        [Route("Delete_material")]
        public async Task<IActionResult> Delete_material(string Id)
        {
            string baseAdd = uri + "/Materials/"; // Include id in the URL
            var token = Request.Cookies["AuthToken"] ?? "";
            token = System.Text.Json.JsonDocument.Parse(token).RootElement.GetProperty("token").GetString();

         
            // add token in the header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    
            // response
            var response = await _httpClient.DeleteAsync(baseAdd+Id);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Delete"] = "Material Id : '" + Id + "' deleted successfully!";
                return NoContent();
            }
            else
            {
                ViewBag.Error = "Error occurred while creating the material.";
                return NotFound();               
            }
        }
        [HttpPut]
        [Route("Modify/{id}")]
        public async Task<IActionResult> Modify(string id, [FromBody] Material material)
        {
            // Validate input
            if (string.IsNullOrEmpty(id) || material == null)
            {
                return BadRequest("Invalid input data.");
            }

            string baseAdd = uri + "/Materials/Modify/"; // Ensure baseAddress is correctly set.
            var tokenCookie = Request.Cookies["AuthToken"] ?? "";

            if (string.IsNullOrEmpty(tokenCookie))
            {
                return Unauthorized("Authorization token is missing.");
            }

            string token;
            try
            {
                token = System.Text.Json.JsonDocument.Parse(tokenCookie).RootElement.GetProperty("token").GetString();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error parsing token: {ex.Message}");
                return Unauthorized("Invalid authorization token.");
            }

            // Serialize the material object
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(material),
                Encoding.UTF8,
                "application/json");

            // Add the token to the Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                // Send the PUT request
                var response = await _httpClient.PutAsync(baseAdd + id, content);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    TempData["Modify"] = "Material '" + material.ColorName + "' updated.";
                    return NoContent();
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"API Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"An error occurred while communicating with the external API.\n{ex}");
            }
        }
        [HttpGet("all_list")]
        public async Task<IActionResult> all_list()
        {
            List<Material> materials = new List<Material>();
            string baseAdd = uri + "/Materials";
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
                    materials = System.Text.Json.JsonSerializer.Deserialize<List<Material>>(jsonResponse, new JsonSerializerOptions
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
            return Ok(materials);
        }
    }
}
