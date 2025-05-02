using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;
using System.Diagnostics;
using System.IO;
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

        [HttpGet]
        public async Task<IActionResult> All_Party(
             string party_,
             string searchTerm = "",
             string sortColumn = "P_Name",
             string sortDirection = "asc",
             int page = 1,
             int pageSize = 10
             )
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryParams["party"] = party_;
            queryParams["searchTerm"] = searchTerm.ToLower();
            queryParams["sortColumn"] = sortColumn;
            queryParams["sortDirection"] = sortDirection;
            queryParams["page"] = page.ToString();
            queryParams["pageSize"] = pageSize.ToString();

            string bassAdd = _httpClient.BaseAddress + $"/Party/AllParty?{queryParams}";
            var response = await _httpClient.GetAsync(bassAdd);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var partyList = JsonSerializer.Deserialize<PartyListViewModel>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(partyList);
            }


            return View();

        }

        [HttpPost("CreatePartyAsync")]
        [Route("Party/CreatePartyAsync")]
        public async Task<IActionResult> CreatePartyAsync([FromBody] Party party)
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
        [Route("Party/search")]
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

        [HttpPost("EditParty")]
        [Route("Party/EditParty")]
        public async Task<IActionResult> Editparty([FromBody] Party party)
        {

            string jsonContent = JsonSerializer.Serialize(party);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            string baseAdd = _httpClient.BaseAddress + "/Party/EditParty";
            var response = await _httpClient.PutAsync(baseAdd, content);
            var message = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(message);



            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Party details updated successfully.";

                return Ok(new
                {
                    success = true,
                    message = message

                });
            }
            else
            {

                return Json(new
                {
                    success = false,
                    message = message
                }); // Fix: Use a single object
            }

        }

        [HttpDelete("DeleteParty/{id}")]
        [Route("Party/DeleteParty/{id}")]
        public async Task<IActionResult> DeleteParty(int id)
        {

            

            string baseAdd = _httpClient.BaseAddress + $"/Party/Delete/{id}";
            var response = await _httpClient.DeleteAsync(baseAdd);
            var message = await response.Content.ReadAsStringAsync();

          


            if (response.IsSuccessStatusCode)
            {
                TempData["Delete"] = message;

                return Ok(new
                {
                    success = true,
                    message = message

                });
            }
            else
            {

                return Json(new
                {
                    success = false,
                    message = message
                }); // Fix: Use a single object
            }

        }

    }
    }


