using Microsoft.AspNetCore.Mvc;

namespace Moksha_App.Controllers
{
    public class SellerBillingController : Controller
    {
        public IActionResult All_bills()
        {
            return View();
        }
    }
}
