using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http;

namespace Moksha_App.Controllers
{
    public class GlobalTokenAuthorizationFilter : IAuthorizationFilter
    {
        
        private readonly HttpClient _client;
        public GlobalTokenAuthorizationFilter(string backend_url)
        {
            _client = new HttpClient();
          
            
            Uri baseAddress = new Uri(backend_url);
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionDescriptor = context.ActionDescriptor;
            var endpointMetadata = actionDescriptor.EndpointMetadata;

            if (endpointMetadata.Any(m => m is AllowAnonymousAttribute))
            {
                return;
            }
            var request = context.HttpContext.Request;
            var token = request.Cookies["AuthToken"]??""; // Retrieve the token from cookies
            // Decode the token if it is JSON encoded
            try
            {
                token = System.Text.Json.JsonDocument.Parse(token).RootElement.GetProperty("token").GetString();


                if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token) || !ValidateToken(token))
                {
                    // If token is invalid or missing, redirect to login page
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                }
            }
            catch 
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
    
        }

        private bool ValidateToken(string _token)
        {
            var token = _token;
            if (string.IsNullOrEmpty(token))
            {
                return false;
                
            }

            string baseAdd = _client.BaseAddress + "/Auth/ValidateToken";

            // Create a new HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, baseAdd)
            {
                // Set Authorization header with Bearer token
                Headers = { { "Authorization", $"Bearer {token}" } }
            };

            // Send the request and get the response
            var response =  _client.Send(requestMessage);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }
}

