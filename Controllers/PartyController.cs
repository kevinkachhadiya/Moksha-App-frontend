using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Moksha_App.Controllers
{
    public class PartyController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _appsettings;
        public PartyController(IConfiguration appsettings)
        {
            _appsettings = appsettings;
            var backend_url = Environment.GetEnvironmentVariable("backend_url")
                 ?? _appsettings["BackendUrl"] ?? "";
            Uri baseAddress = new Uri(backend_url);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }


        public IActionResult All_Party()
        {


            List<Party> l = new List<Party>() {
            new Party
            {

                Id = 1,
                P_Name = "jay",
                P_number = "8928948943",
                p_t = 0,
                IsActive = true
            },
            new Party
            {

                Id = 1,
                P_Name = "jay",
                P_number = "8928948943",
                p_t = 0,
                IsActive = true
            },

            };

            return View();

        }


        [HttpPost("CreatePartyAsync")]
        [Route("Party/CreatePartyAsync")]
        public async Task<IActionResult> CreatePartyAsync([FromBody]Party party)
        {
            string jsonContent = JsonSerializer.Serialize(party);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            string baseAdd = _httpClient.BaseAddress + "/Party/CreateParty";
            var response = await _httpClient.PostAsync(baseAdd, content);
            var message = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(message);
           


            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = $"{party.p_t} added";  

                return Ok(new
                {
                    success = true,
                    message = message

                });
            }
            else
            {

                return Json(new {
                    success = false,
                    message = message
                }); // Fix: Use a single object
            }

        }
    }
}
