using Microsoft.AspNetCore.Mvc;

namespace Moksha_App.Controllers
{
    public class Stock_Management_Controller : Controller
    {
        public IActionResult GetAllStocks()
        {
            return View();
        }
    }
}
