using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;

namespace Moksha_App.Controllers
{
    public class PartyController : Controller
    {
        public IActionResult All_Party()
        {


            List<Party> l = new List<Party>() {
            new Party
            {

                Id = 1,
                P_Name = "jay",
                P_number = 8928948943,
                p_t = 0,
                IsActive = true
            },
            new Party
            {

                Id = 1,
                P_Name = "jay",
                P_number = 8928948943,
                p_t = 0,
                IsActive = true
            },
            
            };

            return View();
        }
    }
}
