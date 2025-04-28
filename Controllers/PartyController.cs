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

        [HttpGet("search")]
        public async Task<IActionResult> search(string search)
        {
            var seachparty = search.ToLower();
            
            
            string baseAdd = _httpClient.BaseAddress + $"/Party/SupplierSearch?search={seachparty}";
  

            var response = await _httpClient.GetAsync(baseAdd);


            Debug.WriteLine(response);

            string jsonResponse = await response.Content.ReadAsStringAsync();



            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, string> p = new Dictionary<string, string>();
                var parties = JsonSerializer.Deserialize<List<Party>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var party in parties)
                {
                    p.Add(party.P_Name, party.P_number);
                }

                if (parties.Count() >= 1)
                {
                    return Ok(new
                    {
                        success = true,
                        data = parties // you can even return the list
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        data = parties // you can even return the list
                    });
                }


            }
            else
            {
                return Ok(new
                {
                    success = false,
                    data = "" // you can even return the list
                });
            }
        }



    }

}
