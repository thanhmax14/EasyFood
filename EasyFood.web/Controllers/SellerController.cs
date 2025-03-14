using Microsoft.AspNetCore.Mvc;

namespace EasyFood.web.Controllers
{
    public class SellerController : Controller
    {
        public IActionResult FeedbackList()
        {
            return View();
        }
    }
}
